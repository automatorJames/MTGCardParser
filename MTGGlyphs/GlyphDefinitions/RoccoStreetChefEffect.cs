namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rocco, Street Chef, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rocco, Street Chef</exampledoc>
/// <examplecapture>you put a +1/+1 counter on target creature and create a food token</examplecapture>
[Dependent]
public class RoccoStreetChefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you put a +1/+1 counter on target creature and create a food token"];
}
