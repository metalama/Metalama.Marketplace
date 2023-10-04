// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.ContinuousIntegration;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

const string projectName = "Metalama.Marketplace";
var productFamily = new ProductFamily( projectName, "2023.0", DevelopmentDependencies.Family );
var repository = new GitHubRepository( projectName );
var ciConfiguration = TeamCityHelper.CreateConfiguration(
    TeamCityHelper.GetProjectId( projectName, "Websites And Business Systems" ),
    "caravela04cloud" );
var dependencyDefinition =
    new DependencyDefinition( productFamily, projectName, "master", null, repository, ciConfiguration, false );

var product = new Product( dependencyDefinition )
{
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering }
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );