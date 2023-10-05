// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Markdig;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using PostSharp.Engineering.BuildTools.Search.Updaters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Typesense;

namespace BuildMetalamaMarketplace;

public class MarketplaceCollectionUpdater : CollectionUpdater
{
    public MarketplaceCollectionUpdater( SearchBackendBase searchBackend )
        : base( searchBackend ) { }

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

        var entries = new List<MarketplaceEntry>();

        if ( settings.Single )
        {
            entries.Add( this.ParseEntry( sourcePath ) );
        }
        else
        {
            foreach ( var entryDirectoryPath in Directory.EnumerateDirectories( sourcePath ) )
            {
                entries.Add( this.ParseEntry( entryDirectoryPath ) );
            }
        }

        await this.Backend.UpsertDocumentsAsync( targetCollection, entries );

        return true;
    }

    private MarketplaceEntry ParseEntry( string entryDirectoryPath )
    {
        (string Encoded, string[] Plain) ReadMarkdown( string markdownFileName )
        {
            var markdownPath = Path.Combine( entryDirectoryPath, markdownFileName );
            var markdown = File.ReadAllText( markdownPath );
            var markdownEncoded = HttpUtility.UrlEncode( markdown );
            var plainText = Markdown.ToPlainText( markdown );
            var plainTextLines = plainText.Split( '\n' ).Select( s => s.Trim() ).Where( s => s.Length > 0 ).ToArray();
            
            return (markdownEncoded, plainTextLines);
        }
        
        var entryJsonPath = Path.Combine( entryDirectoryPath, $"{Path.GetFileName( entryDirectoryPath )}.json" );
        var entryJson = File.ReadAllText( entryJsonPath );
        var entry = JsonSerializer.Deserialize<MarketplaceEntry>( entryJson ) ??
                    throw new InvalidOperationException( $"Failed to deserialize '{entryDirectoryPath}'." );
        var summary = ReadMarkdown( entry.Summary );
        entry.Summary = summary.Encoded;
        entry.SummaryText = summary.Plain;

        foreach ( var aspect in entry.Aspects )
        {
            var description = ReadMarkdown( aspect.Description );
            aspect.Description = description.Encoded;
            aspect.DescriptionText = description.Plain;
        }

        return entry;
    }

    public override Schema CreateSchema( string collectionName ) =>
        CollectionSchemaFactory.CreateSchema<MarketplaceEntry>( collectionName );
}