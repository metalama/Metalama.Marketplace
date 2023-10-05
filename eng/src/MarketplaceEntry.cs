// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

public class MarketplaceEntry
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }

    [JsonPropertyName( "kind" )]
    public string Kind { get; set; }

    [JsonPropertyName( "categories" )]
    public string[] Categories { get; set; }
}