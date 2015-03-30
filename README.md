# NbpClient [![Build status](https://ci.appveyor.com/api/projects/status/m0brfye6kv3qlelj/branch/master?retina=true)](https://ci.appveyor.com/project/mgibas/nbpclient/branch/master)

National Bank of Poland currency exchange rate client.

NuGet
====
```
Install-Package NbpClient
Install-Package NbpClient.Autofac
Install-Package NbpClient.Ninject
```
Features
====
* Checking exchange rate avaibility
* Getting a/b/c/h exchange rate for given date (ie. from http://www.nbp.pl/kursy/xml/ )

Usages
====
Autofac Extension
```csharp
builder.RegisterNbpClient();
```

Ninject Extension
```csharp
kernel.RegisterNbpClient();
```

Basic usage of getting average exchange rate (the 'A' table)
```csharp
var client = container.Resolve<INbpClient>();
var availableDates = client.GetExchangeRateAvailableDates<AverageExchangeRate>();
var rate = client.GetExchangeRates<AverageExchangeRate>(availableDates.First());
```
