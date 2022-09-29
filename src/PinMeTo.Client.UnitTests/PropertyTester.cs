using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit.Sdk;

namespace TheCodePatch.PinMeTo.Client.UnitTests;

/// <summary>
///     Provides methods to test property values on a collection of items.
/// </summary>
internal class PropertyTester<TItems>
{
    private readonly string _path;
    private readonly List<string> _errors;
    private readonly IList<TItems> _items;

    public PropertyTester(IList<TItems> items) : this(items, "") { }

    private PropertyTester(IList<TItems> items, string path)
    {
        _items = items;
        _errors = new();
        _path = path;
    }

    public PropertyTester<TItems> NoneShouldEqual<TOther>(
        Expression<Func<TItems, TOther?>> selector,
        TOther disallowedValue
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);
        if (_items.Any(x => Equals(disallowedValue, selectFunction(x))))
        {
            _errors.Add(
                $"Found disallowedValue value {disallowedValue} among the {_path}{propertyName} values"
            );
        }

        return this;
    }

    public PropertyTester<TItems> NoneShouldBeNullOrWhiteSpace(
        Expression<Func<TItems, string?>> selector
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);
        if (_items.Any(x => string.IsNullOrWhiteSpace(selectFunction(x))))
        {
            _errors.Add($"Found {_path}{propertyName} values that were null or whitespace");
        }

        return this;
    }

    public void ShouldHaveNoErrors()
    {
        if (_errors.Any())
        {
            throw new XunitException(
                "The following errors occurred\n  " + string.Join("\n  ", _errors)
            );
        }
    }

    public PropertyTester<TItems> NoneShouldBeNull<TValue>(
        Expression<Func<TItems, IEnumerable<TValue>?>> selector,
        bool allowEmptyEnumerables,
        Action<PropertyTester<TValue>>? inner = null
    )
    {
        this.NoneShouldBeNull(selector);
        var (selectFunction, propertyName) = GetSelectFunction(selector);
        var nonNullValues = _items
            .Select(selectFunction)
            .Where(x => null != x)
            .Cast<IEnumerable<TValue>>()
            .ToList();
        if (false == allowEmptyEnumerables && nonNullValues.Any(l => false == l.Any()))
        {
            _errors.Add($"Found {_path}{propertyName} values that were empty enumerables");
        }

        if (null != inner)
        {
            var allListedItems = nonNullValues.SelectMany(x => x).ToList();
            if (false == allListedItems.Any())
            {
                _errors.Add(
                    $"There were no items to run inner checks on for {_path}{propertyName}."
                );
            }
            var innerTester = GetNestedTester(allListedItems, propertyName);
            inner.Invoke(innerTester);
            _errors.AddRange(innerTester._errors);
        }

        return this;
    }

    public PropertyTester<TItems> SomeShouldNotBeNull<TValue>(
        Expression<Func<TItems, TValue>> selector,
        Action<PropertyTester<TValue>>? inner = null
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);

        var allNonNullValues = _items.Select(x => selectFunction(x)).Where(x => null != x).ToList();

        if (false == allNonNullValues.Any())
        {
            _errors.Add($"Found no {_path}{propertyName} values that were not null");
        }

        if (null != inner)
        {
            var innerTester = GetNestedTester(allNonNullValues, propertyName);
            inner.Invoke(innerTester);
            _errors.AddRange(innerTester._errors);
        }

        return this;
    }

    public PropertyTester<TItems> SomeShouldBeNull<TValue>(
        Expression<Func<TItems, TValue?>> selector,
        Action<PropertyTester<TValue>>? inner = null
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);

        if (false == _items.Any(x => selectFunction(x) == null))
        {
            _errors.Add($"Found no {_path}{propertyName} values that were null");
        }

        if (null != inner)
        {
            var allNonNullValues = _items
                .Select(x => selectFunction(x))
                .Where(x => null != x)
                .ToList();
            if (allNonNullValues.Any())
            {
                var innerTester = GetNestedTester(allNonNullValues, propertyName);
                inner.Invoke(innerTester!);
                _errors.AddRange(innerTester._errors);
            }
            else
            {
                _errors.Add($"Found no {_path}{propertyName} values that were not null");
            }
        }

        return this;
    }

    public PropertyTester<TItems> NoneShouldBeNull<TValue>(
        Expression<Func<TItems, TValue>> selector,
        Action<PropertyTester<TValue>>? inner = null
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);
        if (_items.Any(x => null == selectFunction(x)))
        {
            _errors.Add($"Found {_path}{propertyName} values that were null");
        }

        if (null != inner)
        {
            var allValues = _items.Select(x => selectFunction(x)).Where(x => null != x).ToList();
            var innerTester = GetNestedTester(allValues, propertyName);
            inner.Invoke(innerTester);
            _errors.AddRange(innerTester._errors);
        }

        return this;
    }

    private PropertyTester<T> GetNestedTester<T>(IList<T> allValues, string propertyName)
    {
        return new PropertyTester<T>(allValues, $"{_path}{propertyName}.");
    }

    public PropertyTester<TItems> SomeShouldNotBeNullOrWhiteSpace(
        Expression<Func<TItems, string?>> selector
    )
    {
        var (selectFunction, propertyName) = GetSelectFunction(selector);
        if (false == _items.Any(x => false == string.IsNullOrWhiteSpace(selectFunction(x))))
        {
            _errors.Add($"Found no {_path}{propertyName} values that were populated");
        }

        return this;
    }

    private (Func<TItems, TValue> selectFunction, string propertyName) GetSelectFunction<TValue>(
        Expression<Func<TItems, TValue>> expr
    )
    {
        var propertyName = ((MemberExpression)expr.Body).Member.Name;
        var selectMethod = expr.Compile();
        return (selectMethod, propertyName);
    }
}
