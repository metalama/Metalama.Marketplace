// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

#nullable disable

public class Aspect
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }
    
    [JsonPropertyName( "documentationUrl" )]
    public string DocumentationUrl { get; set; }
    
    [JsonPropertyName( "description" )]
    public string Description { get; set; }
    
    [JsonPropertyName( "descriptionText" )]
    public string DescriptionText { get; set; }
}

public class AspectGroup
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }
    
    [JsonPropertyName( "aspects" )]
    public Aspect[] Aspects { get; set; }
} 