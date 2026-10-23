namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Painter's Servant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Painter's Servant</exampledoc>
[TokenizationOrder(-1)]
public class PaintersServantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all cards that aren't on the battlefield, spells, and permanents are the chosen color in addition to their other colors"];
}
