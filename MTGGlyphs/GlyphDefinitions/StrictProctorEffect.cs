namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Strict Proctor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Strict Proctor</exampledoc>
[Dependent]
public class StrictProctorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter that ability unless its controller pays {2}"];
}
