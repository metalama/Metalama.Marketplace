// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using BuildMetalamaMarketplace;
using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Triggers;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using PostSharp.Engineering.BuildTools.Search;
using Spectre.Console.Cli;

static BuildConfigurationInfo RemoveFromTeamCity( BuildConfigurationInfo c ) => c with
{
    ExportsToTeamCityBuild = false,
    ExportsToTeamCityDeploy = false,
    ExportsToTeamCityDeployWithoutDependencies = false
};

var product = new Product( BusinessSystemsDependencies.MetalamaMarketplace )
{
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering },
    Configurations = Product.DefaultConfigurations
        .WithValue( BuildConfiguration.Debug, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Release, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Public, RemoveFromTeamCity ),
    Extensions = new ProductExtension[]
    {
        /*
        new UpdateSearchProductExtension<UpdateMarketplaceCommand>(
            "https://0fpg9nu41dat6boep.a1.typesense.net",
            "metalama-marketplace",
            "entries",
            customBuildConfigurationName: "Deploy [Public]",
            buildTriggers: new(null, null, new IBuildTrigger[] { new NightlyBuildTrigger( 0, false ) }) )
            */
    }
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );
commandApp.Configure( config => config.Settings.PropagateExceptions = true );

return commandApp.Run( args );