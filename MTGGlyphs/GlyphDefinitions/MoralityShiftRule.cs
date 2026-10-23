namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Morality Shift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Morality Shift</exampledoc>
[TokenizationOrder(-1)]
public class MoralityShiftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your graveyard and library"];
}
