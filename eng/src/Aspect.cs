// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

#nullable disable

public class Aspect
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }
    
    [JsonPropertyName( "doc-url" )]
    public string DocumentationUrl { get; set; }
    
    [JsonPropertyName( "description" )]
    public string Description { get; set; }
    
    [JsonPropertyName( "description-text" )]
    public string[] DescriptionText { get; set; }
}