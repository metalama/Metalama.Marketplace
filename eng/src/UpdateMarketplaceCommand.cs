// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Updaters;

namespace BuildMetalamaMarketplace;

public class UpdateMarketplaceCommand : UpdateSearchCommand
{
    protected override CollectionUpdater CreateUpdater( SearchBackend backend ) =>
        new MarketplaceCollectionUpdater( backend );
}