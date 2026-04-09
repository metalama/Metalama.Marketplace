// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using BuildMetalamaMarketplace;
using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.ContinuousIntegration.Triggers;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using PostSharp.Engineering.BuildTools.Docker;
using PostSharp.Engineering.BuildTools.Search;

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
    OverriddenBuildAgentRequirements = new ContainerRequirements( ContainerHostKind.Windows )
    {
        Components =
        [
            new DotNetComponent( PreferredVersions.DotNetSdk.V_8_0, DotNetComponentKind.Sdk ),
        ]
    },
    DotNetSdkVersion = new DotNetSdkVersion( PreferredVersions.DotNetSdk.V_8_0 ) { AllowPrerelease = true },
    Configurations = Product.DefaultConfigurations
        .WithValue( BuildConfiguration.Debug, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Release, RemoveFromTeamCity )
        .WithValue( BuildConfiguration.Public, RemoveFromTeamCity ),
    Extensions =
    [
        new UpdateSearchProductExtension(
            "https://typesense.postsharp.net",
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