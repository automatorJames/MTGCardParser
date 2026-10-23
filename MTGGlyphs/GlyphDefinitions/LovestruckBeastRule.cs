namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lovestruck Beast // Heart's Desire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lovestruck Beast // Heart's Desire</exampledoc>
[TokenizationOrder(-1)]
public class LovestruckBeastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't attack unless you control a 1/1 creature"];
}
