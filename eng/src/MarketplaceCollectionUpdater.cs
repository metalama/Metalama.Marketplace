// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using PostSharp.Engineering.BuildTools.Search.Updaters;
using PostSharp.Engineering.BuildTools.Utilities;
using System.Threading.Tasks;
using Typesense;

namespace BuildMetalamaMarketplace;

public class MarketplaceCollectionUpdater : CollectionUpdater
{
    public MarketplaceCollectionUpdater(SearchBackend searchBackend) : base(searchBackend)
    {
    }

    public override Task<bool> UpdateAsync(
        ConsoleHelper console,
        UpdateSearchCommandSettings settings,
        string targetCollection )
    {
        return Task.FromResult( false );
    }

    public override Schema CreateSchema( string collectionName ) =>
        CollectionSchemaFactory.CreateSchema<MarketplaceEntry>( collectionName );
}