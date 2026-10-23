namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Fallen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Fallen</exampledoc>
[Dependent]
public class TheFallenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals 1 damage to each opponent and planeswalker it has dealt damage to this game"];
}
