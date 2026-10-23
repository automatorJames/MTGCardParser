namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ragemonger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ragemonger</exampledoc>
[Dependent]
public class RagemongerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if you cast a minotaur spell with mana cost {2}{r}, it costs {2} to cast"];
}
