// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Search;
using PostSharp.Engineering.BuildTools.Search.Backends;
using PostSharp.Engineering.BuildTools.Search.Updaters;

namespace BuildMetalamaMarketplace;

public class UpdateMarketplaceCommand : UpdateSearchCommandBase
{
    protected override CollectionUpdater CreateUpdater( SearchBackendBase backend ) =>
        new MarketplaceCollectionUpdater( backend );
}