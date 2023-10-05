// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

public class MarketplaceEntry
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }

    [JsonPropertyName( "kind" )]
    [Facet]
    public string Kind { get; set; }

    [JsonPropertyName( "categories" )]
    [Facet]
    public string[] Categories { get; set; }

    [JsonPropertyName( "author" )]
    public string Author { get; set; }
    
    [JsonPropertyName( "quality" )]
    [Facet]
    public string Quality { get; set; }
    
    [JsonPropertyName( "license" )]
    [Facet]
    public string License { get; set; }
    
    [JsonPropertyName( "src-url" )]
    public string SourceUrl { get; set; }
    
    [JsonPropertyName( "doc-url" )]
    public string DocumentationUrl { get; set; }
    
    [JsonPropertyName( "summary" )]
    public string Summary { get; set; }
    
    [JsonPropertyName( "summary-text" )]
    public string[] SummaryText { get; set; }
    
    [JsonPropertyName( "keywords" )]
    public string[] Keywords { get; set; }
    
    [JsonPropertyName( "aspects" )]
    public Aspect[] Aspects { get; set; }
}