namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dragon's Disciple, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragon's Disciple</exampledoc>
[TokenizationOrder(-1)]
public class DragonsDiscipleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do or if you control a dragon, this creature enters with a +1/+1 counter on it"];
}
