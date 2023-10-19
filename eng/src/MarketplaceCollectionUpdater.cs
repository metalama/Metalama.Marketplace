// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using PostSharp.Engineering.BuildTools.Search.Updaters;
using ReadSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Typesense;

namespace BuildMetalamaMarketplace;

public class MarketplaceCollectionUpdater : CollectionUpdater
{
    private static readonly XmlSerializer _xmlSerializer = new XmlSerializer( typeof(AspectLibrary) );

    public MarketplaceCollectionUpdater( SearchBackendBase searchBackend )
        : base( searchBackend )
    {
    }

    public override async Task<bool> UpdateAsync(
        BuildContext context,
        UpdateSearchCommandSettings settings,
        string targetCollection )
    {
        var sourcePath = settings.SourceUrl;

        if ( !Path.IsPathRooted( sourcePath ) )
        {
            sourcePath = Path.Combine( context.RepoDirectory, sourcePath );
        }

        var aspectLibraries = new List<AspectLibrary>();

        if ( settings.Single )
        {
            throw new NotSupportedException();
        }

        // Parse all libraries.
        foreach ( var aspectLibraryXmlPath in Directory.EnumerateFiles(
                     sourcePath,
                     "*.xml",
                     SearchOption.TopDirectoryOnly ) )
        {
            aspectLibraries.Add( ParseAspectLibrary( aspectLibraryXmlPath ) );
        }
        
        // Download statistics.
        async Task GetDownloadStatistics(AspectLibrary aspectLibrary)
        {
            var packageRegex = new Regex(@"https://www\.nuget\.org/packages/(?<package>[^/]+)/?");
            var packageMatch = packageRegex.Match(aspectLibrary.PackageUrl);

            if (packageMatch.Success)
            {
                var packageName = packageMatch.Groups["package"].Value;

                context.Console.WriteMessage($"Getting download statistics for {packageName}.");

                int? downloadCounts;

                try
                {
                    downloadCounts = await NuGetGalleryHelper.GetDownloadCountsAsync(packageName, context);
                }
                catch (Exception e)
                {
                    context.Console.WriteError(e.ToString());
                    downloadCounts = null;
                }

                if (downloadCounts == null)
                {
                    context.Console.WriteWarning($"Cannot get the download count of '{packageName}'.");
                }

                aspectLibrary.DownloadCounts = downloadCounts ?? 0;
            }
            else
            {
                context.Console.WriteWarning($"{aspectLibrary.PackageUrl} is not a correct NuGet URL.");
            }
        }
        
        var tasks = aspectLibraries.Where( l => !string.IsNullOrEmpty( l.PackageUrl ) ).Select( GetDownloadStatistics );
        await Task.WhenAll( tasks );
        
        // Compute relative popularity and rank.
        var maxDownloads = aspectLibraries.Max( l => l.DownloadCounts );
        foreach ( var aspectLibrary in aspectLibraries )
        {
            aspectLibrary.Complete(maxDownloads);
        }

        await this.Backend.UpsertDocumentsAsync( targetCollection, aspectLibraries );

        return true;
    }

    private static AspectLibrary ParseAspectLibrary( string aspectLibraryXmlPath )
    {
        AspectLibrary aspectLibrary;

        using ( var aspectLibraryXmlStream = File.OpenRead( aspectLibraryXmlPath ) )
        {
            using var aspectLibraryXmlReader = new XmlTextReader( aspectLibraryXmlStream );
            aspectLibrary = (AspectLibrary) _xmlSerializer.Deserialize( aspectLibraryXmlReader )!;
        }
        
        TrimStrings( aspectLibrary );

        aspectLibrary.Description ??= "";
        aspectLibrary.AspectGroups ??= new[] { new AspectGroup { Aspects = Array.Empty<Aspect>() } };
        aspectLibrary.PackageUrl ??= "";
        
        aspectLibrary.SummaryText = HtmlUtilities.ConvertToPlainText( aspectLibrary.Summary);
        aspectLibrary.DescriptionText = HtmlUtilities.ConvertToPlainText( aspectLibrary.Description );

        foreach ( var aspect in aspectLibrary.AspectGroups.Where( g => g.Aspects != null ).SelectMany( x => x.Aspects ) )
        {
            aspect.DescriptionText = HtmlUtilities.ConvertToPlainText( aspect.Description ?? "" );
        }
        
        return aspectLibrary;

        static void TrimStrings( object o )
        {
            foreach ( var property in o.GetType().GetProperties() )
            {
                if ( property.PropertyType.IsPrimitive )
                {
                    continue;
                }
                
                if ( property.PropertyType.IsArray )
                {
                    var elementType = property.PropertyType.GetElementType()!;
                    
                    if ( elementType.IsPrimitive )
                    {
                        continue;
                    }

                    var value = property.GetValue( o );

                    if ( value == null )
                    {
                        continue;
                    }

                    if ( elementType == typeof(string) )
                    {
                        var trimmedStrings = ((IEnumerable) value).Cast<string>()
                            .Select( s => s.Trim() ).ToArray();

                        property.SetValue( o, trimmedStrings );
                    }
                    else
                    {
                        foreach ( var element in (IEnumerable) value )
                        {
                            if ( element != null )
                            {
                                TrimStrings( element );
                            }
                        }
                    }
                }
                else if (property.PropertyType == typeof(string))
                {
                    var value = (string?) property.GetValue( o );

                    if ( value != null )
                    {
                        property.SetValue( o, value.Trim() );
                    }
                }
                else
                {
                    var value = property.GetValue( o );

                    if ( value != null )
                    {
                        TrimStrings( value );
                    }
                }
            }
        }
    }

    public override Schema CreateSchema( string collectionName ) =>
        CollectionSchemaFactory.CreateSchema<AspectLibrary>( collectionName );
}