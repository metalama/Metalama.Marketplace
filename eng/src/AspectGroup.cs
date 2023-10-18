// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

#nullable disable

public class AspectGroup
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }
    
    [JsonPropertyName( "aspects" )]
    public Aspect[] Aspects { get; set; }
}