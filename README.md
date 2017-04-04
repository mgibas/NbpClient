# NbpClient

<p align="center">
    <a href="https://ci.appveyor.com/project/mgibas/nbpclient/branch/master">
        <img src="https://ci.appveyor.com/api/projects/status/github/mgibas/nbpclient?branch=master&svg=true"></img>
    </a>
    <a href="https://www.gitcheese.com/donate/users/530319/repos/32922611">
        <img src="https://s3.amazonaws.com/gitcheese-ui-master/images/badge.svg"></img>
    </a>
    <a href="https://www.nuget.org/packages/NbpClient/">
        <img src="https://img.shields.io/nuget/v/NbpClient.svg?style=flat-square"></img>
    </a>
</p>

===

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
