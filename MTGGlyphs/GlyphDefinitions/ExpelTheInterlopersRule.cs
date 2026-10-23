namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Expel the Interlopers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Expel the Interlopers</exampledoc>
[TokenizationOrder(-1)]
public class ExpelTheInterlopersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures with power greater than or equal to the chosen number"];
}
