namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Throne of Empires, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Throne of Empires</exampledoc>
[TokenizationOrder(-1)]
public class ThroneOfEmpiresRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create five of those tokens instead if you control artifacts named crown of empires and scepter of empires"];
}
