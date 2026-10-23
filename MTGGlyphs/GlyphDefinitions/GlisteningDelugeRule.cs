namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glistening Deluge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glistening Deluge</exampledoc>
[TokenizationOrder(-1)]
public class GlisteningDelugeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures that are green and/or white get an additional -2/-2 until end of turn"];
}
