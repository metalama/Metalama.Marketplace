// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using BuildMetalamaMarketplace;
using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Triggers;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using PostSharp.Engineering.BuildTools.Search;
using Spectre.Console.Cli;

static BuildConfigurationInfo RemoveFromTeamCity( BuildConfigurationInfo c )
{
    return c with
    {
        ExportsToTeamCityBuild = false,
        ExportsToTeamCityDeploy = false,
        ExportsToTeamCityDeployWithoutDependencies = false
    };
}

var product = new Product( BusinessSystemsDependencies.MetalamaMarketplace )
{
    Dependencies = [DevelopmentDependencies.PostSharpEngineering],
    Configurations = Product.DefaultConfigurations
        .WithValue( BuildConfiguration.Debug, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Release, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Public, RemoveFromTeamCity ),
    Extensions =
    [
        new UpdateSearchProductExtension(
            "https://0fpg9nu41dat6boep.a1.typesense.net",
            "metalama-marketplace",
            "entries",
            backend => new MarketplaceCollectionUpdater( backend ),
            customBuildConfigurationName: "Deploy [Public]",
            buildTriggers: new ConfigurationSpecific<IBuildTrigger[]?>( null, null,
                [new NightlyBuildTrigger( 0, false )] ) )
    ]
};

var commandApp = new EngineeringApp(product);
commandApp.Configure( config => config.Settings.PropagateExceptions = true );

return commandApp.Run( args );