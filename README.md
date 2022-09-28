# pinmeto-client
A .NET client for PinMeTo

# Usage
* Import NuGet package PinMeTo.Client
* Implement a record representing the _custom data_ of your PinMeToAccount.
* Add the client to your DI container by using one of the extension methods in the `TheCodePatch.PinMeTo.Client` namespace and apply the appropriate configuration.
* Resolve the `ILocationsService<TYourCustomData>` and use it to interact with the API. 
 
Each step is explained in the sections below.

## Import NuGet package PinMeTo.Client
`dotnet add package PinMeTo.Client`

## Custom data
You need to implement a model of the _custom data_ configured in your PinMeTo account.
This is a simple POCO whose properties have the same name as your custom data properties,
or are mapped using `System.Text.Json.Serialization.JsonPropertyNameAttribute` attributes.

Example:
```csharp
public record MyCustomData
{
    [JsonPropertyName("premstore")]
    public bool IsPremiumStore { get; set; }
}
```

## Add the client to your DI container
You add the client to your DI container using one of two extension methods in the `TheCodePatch.PinMeTo.Client` namespace.

### If you want your connection details in a config file
Add the following section to your configuration:
```json
"PinMeToClient": {
  "ApiBaseAddress": "https://api.test.pinmeto.com",
  "AppId": "insert app id",
  "AppSecret": "insert app secret",
  "AccountId": "insert account id"
},
```
And use the following method on your IServiceCollection during DI container setup:
```csharp
serviceCollection.AddPinMeToClient<MyCustomData>(conf.GetSection("PinMeToClient"))
```

### If you want to configure the client in code
Use the following method on your IServiceCollection during DI container setup:
```csharp
serviceCollection.AddPinMeToClient<MyCustomData>(
    c =>
    {
        c.AccountId = "my account id";
        c.AppId = "my app id";
        c.AppSecret = "my app secret";
        c.ApiBaseAddress = new Uri("https://api.test.pinmeto.com");
    }
)
```

## Resolve the locations service

Example:
```csharp
public class MyController: Controller
{
    private ILocationsService<MyCustomData> _locationsService;
    
    public MyController(ILocationsService<MyCustomData> locationsService)
    {
        _locationsService = locationsService;
    }
    
    public Task<LocationDetails<MyCustomData>> GetLocation(string storeId)
    {
        return _locationsService.Get(storeId);
    }
}
```


# Contributing

Please use [CSharpier](https://csharpier.com/) with Run on save activated to keep formatting consistent.

To run unit tests, you need to have an `appsettings.private.json` file in the unit test project. 
Copy the already existing file `appsettings.private.json.example` and remove the `.example` suffix. 
Then enter the connection details for your PinMeTo account (note! don't use a production account, as 
the test add new stores to the account.