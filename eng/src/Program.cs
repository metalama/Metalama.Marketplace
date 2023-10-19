// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using BuildMetalamaMarketplace;
using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.ContinuousIntegration;
using PostSharp.Engineering.BuildTools.ContinuousIntegration.Model;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using PostSharp.Engineering.BuildTools.Search;
using Spectre.Console.Cli;

const string projectName = "Metalama.Marketplace";
var productFamily = new ProductFamily( projectName, "2023.0", DevelopmentDependencies.Family );
var repository = new GitHubRepository( projectName );
var ciConfiguration = TeamCityHelper.CreateConfiguration(
    new TeamCityProjectId( "MetalamaMarketplace", "WebsitesAndBusinessSystems" ),
    "caravela04cloud" );
var dependencyDefinition =
    new DependencyDefinition( productFamily, projectName, "master", null, repository, ciConfiguration, false );

static BuildConfigurationInfo RemoveFromTeamCity( BuildConfigurationInfo c ) => c with
{
    ExportsToTeamCityBuild = false,
    ExportsToTeamCityDeploy = false,
    ExportsToTeamCityDeployWithoutDependencies = false
};

var product = new Product( dependencyDefinition )
{
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering },
    Configurations = Product.DefaultConfigurations
        .WithValue( BuildConfiguration.Debug, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Release, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Public, RemoveFromTeamCity ),
    Extensions = new ProductExtension[]
    {
        new UpdateSearchProductExtension<UpdateMarketplaceCommand>(
            "https://0fpg9nu41dat6boep.a1.typesense.net",
            "metalama-marketplace",
            "entries",
            customBuildConfigurationName: "Deploy [Public]" )
    }
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );
commandApp.Configure( config => config.Settings.PropagateExceptions = true );

return commandApp.Run( args );