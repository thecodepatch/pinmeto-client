# pinmeto-client
A .NET client for PinMeTo

# Usage
* Import NuGet package PinMeTo.Client
* Add the client to your DI container by using one of the extension methods in the `TheCodePatch.PinMeTo.Client` namespace (see sections below).
* Resolve the `ILocationsService` and use it to interact with the API.

## If you want your connection details in a config file
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
serviceCollection.AddPinMeToClient(conf.GetSection("PinMeToClient"))
```

## If you want to configure the client in code
Use the following method on your IServiceCollection during DI container setup:
```csharp
serviceCollection.AddPinMeToClient(
    c =>
    {
        c.AccountId = "my account id";
        c.AppId = "my app id";
        c.AppSecret = "my app secret";
        c.ApiBaseAddress = new Uri("https://api.test.pinmeto.com");
    }
)
```

# Contributing

Please use [CSharpier](https://csharpier.com/) with Run on save activated to keep formatting consistent.

To run unit tests, you need to have an `appsettings.private.json` file in the unit test project. 
Copy the already existing file `appsettings.private.json.example` and remove the `.example` suffix. 
Then enter the connection details for your PinMeTo account (note! don't use a production account, as 
the test add new stores to the account.