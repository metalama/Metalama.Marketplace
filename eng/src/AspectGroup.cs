using System.Text.Json.Serialization;

namespace BuildMetalamaMarketplace;

public class AspectGroup
{
    [JsonPropertyName( "name" )]
    public string Name { get; set; }
    
    [JsonPropertyName( "aspects" )]
    public Aspect[] Aspects { get; set; }
}