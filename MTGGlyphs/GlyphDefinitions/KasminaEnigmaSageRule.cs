namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kasmina, Enigma Sage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kasmina, Enigma Sage</exampledoc>
[TokenizationOrder(-1)]
public class KasminaEnigmaSageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other planeswalker you control has the loyalty abilities of kasmina"];
}
