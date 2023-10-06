// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using PostSharp.Engineering.BuildTools.Search.Updaters;
using ReadSharp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            aspectLibraries.Add( ParseAspectLibrary( sourcePath ) );
        }
        else
        {
            foreach ( var aspectLibraryXmlPath in Directory.EnumerateFiles(
                         sourcePath,
                         "*.xml",
                         SearchOption.TopDirectoryOnly ) )
            {
                aspectLibraries.Add( ParseAspectLibrary( aspectLibraryXmlPath ) );
            }
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

        aspectLibrary.SummaryText = ReadHtml( aspectLibrary.Summary );

        foreach ( var aspect in aspectLibrary.Aspects )
        {
            aspect.DescriptionText = ReadHtml( aspect.Description );
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

        static string[] ReadHtml( string html )
        {
            var plainText = HtmlUtilities.ConvertToPlainText( html );
            var plainTextLines = plainText.Split( '\n' ).Select( s => s.Trim() ).Where( s => s.Length > 0 ).ToArray();
            return plainTextLines;
        }
    }

    public override Schema CreateSchema( string collectionName ) =>
        CollectionSchemaFactory.CreateSchema<AspectLibrary>( collectionName );
}