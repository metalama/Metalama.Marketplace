// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools.Search.Backends.Typesense;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace BuildMetalamaMarketplace;

#nullable disable

public class AspectLibrary
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }

    [JsonPropertyName( "kind" )]
    [Facet]
    public string Kind { get; set; }

    [XmlArray("Categories")]
    [XmlArrayItem("Category")]
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
    
    [JsonPropertyName( "packageUrl" )]
    public string PackageUrl { get; set; }
    
    [JsonPropertyName( "sourceUrl" )]
    public string SourceUrl { get; set; }
    
    [JsonPropertyName( "documentationUrl" )]
    public string DocumentationUrl { get; set; }
    
    [JsonPropertyName( "summary" )]
    public string Summary { get; set; }
    
    [JsonPropertyName( "summaryText" )]
    public string SummaryText { get; set; }
    
    [JsonPropertyName( "description" )]
    public string Description { get; set; }
    
    [JsonPropertyName( "descriptionText" )]
    public string DescriptionText { get; set; }
    
    [XmlArray("Keywords")]
    [XmlArrayItem("Keyword")]
    [JsonPropertyName( "keywords" )]
    public string[] Keywords { get; set; }
    
    [JsonPropertyName( "aspectGroups" )]
    public AspectGroup[] AspectGroups { get; set; }
}