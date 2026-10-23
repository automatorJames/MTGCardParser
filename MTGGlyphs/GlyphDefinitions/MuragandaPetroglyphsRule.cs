namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Muraganda Petroglyphs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Muraganda Petroglyphs</exampledoc>
[TokenizationOrder(-1)]
public class MuragandaPetroglyphsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures with no abilities get +2/+2"];
}
