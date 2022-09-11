# Board libraries for TinyCLR OS.

[![NuGet Status](http://img.shields.io/nuget/v/Bytewizer.TinyCLR.Portal.svg?style=flat&logo=nuget)](https://www.nuget.org/packages?q=bytewizer.tinyclr.boards)
[![Release](https://github.com/bytewizer/boards/actions/workflows/release.yml/badge.svg)](https://github.com/bytewizer/boards/actions/workflows/release.yml)
[![Build](https://github.com/bytewizer/boards/actions/workflows/actions.yml/badge.svg)](https://github.com/bytewizer/boards/actions/workflows/actions.yml)

This repo contains several single board computer libraries built for [GHI Electronics TinyCLR OS](https://www.ghielectronics.com/).

## Give a Star! :star:

If you like or are using this project to start your solution, please give it a star. Thanks!

# Wireless Application
Install one of the release package from [NuGet](https://www.nuget.org/packages?q=bytewizer.tinyclr.boards) or using the Package Manager Console matching your board :
```powershell
PM> Install-Package Bytewizer.TinyCLR.Boards.Bit
PM> Install-Package Bytewizer.TinyCLR.Boards.Duino
PM> Install-Package Bytewizer.TinyCLR.Boards.Feather
PM> Install-Package Bytewizer.TinyCLR.Boards.Portal
PM> Install-Package Bytewizer.TinyCLR.Boards.Switch
PM> Install-Package Bytewizer.TinyCLR.Boards.SC20100
PM> Install-Package Bytewizer.TinyCLR.Boards.SC20260
```

```csharp
using Bytewizer.TinyCLR.Boards;
using Bytewizer.TinyCLR.Hosting;

namespace Bytewizer.Playground.Moxi
{
    internal class Program
    {
        static void Main()
        {
            IHost host = HostBoard.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddWireless("ssid", "password");
                    services.AddNetworkTime();

                    services.AddHostedService(typeof(NetworkStatusService));

                }).Build();

            host.Run();
        }
    }
}
```

# Wireless Application with json configuration file on sd card

```powershell
PM> Install-Package Bytewizer.TinyCLR.Hosting.Configuration.Json
```

```json
{
    "wireless": {
        "ssid": "ssid",
        "psk": "passworkd"
    },
    "logging": {
        "log-level": "Information"
    },
    "timezone": {
        "offset": "-25200"
    }
}
```

```csharp
internal class Program
{
    static void Main()
    {
        IHost host = HostBoard.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", optional:false);
            })
            .ConfigureServices(services =>
            {
                services.AddWireless();
                services.AddNetworkTime();

                services.AddHostedService(typeof(NetworkStatusService));

            }).Build();

        host.Run();
    }
}

public static class DefaultServiceCollectionExtension
{
    public static IServiceCollection AddWireless(this IServiceCollection services)
    {
        return services.AddWireless(null, null);
    }
}

public static class DefaultConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddJsonFile(
        this IConfigurationBuilder builder, string path, bool optional)
    {
        try
        {
            var controller = StorageController.GetDefault();
            var driveProvider = FileSystem.Mount(controller.Hdc);

            return builder.AddJsonFile(driveProvider, path, optional);
        }
        catch
        {
            throw new InvalidOperationException("Failed to mount sd card.");
        }
    }
}
```

## Requirements

Software: <a href="https://visualstudio.microsoft.com/downloads/">Visual Studio 2019/2022</a> and <a href="https://www.ghielectronics.com/">GHI Electronics TinyCLR OS</a>.  

## Nuget Packages

Install releases package from [NuGet](https://www.nuget.org/packages?q=bytewizer). Development build packages are available as [Github Packages](https://github.com/bytewizer?tab=packages).

## Continuous Integration

**main** :: This is the branch containing the latest release build. No contributions should be made directly to this branch. The development branch will periodically be merged to the main branch, and be released to [NuGet](https://www.nuget.org/packages?q=bytewizer).

**develop** :: This is the development branch to which contributions should be proposed by contributors as pull requests. Development build packages are available as [Github Packages](https://github.com/bytewizer?tab=packages).

## Contributions

Contributions to this project are always welcome. Please consider forking this project on GitHub and sending a pull request to get your improvements added to the original project.

## Disclaimer

All source, documentation, instructions and products of this project are provided as-is without warranty. No liability is accepted for any damages, data loss or costs incurred by its use.