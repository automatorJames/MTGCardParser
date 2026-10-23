namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Deadbridge Chant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deadbridge Chant</exampledoc>
[Dependent]
public class DeadbridgeChantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a card at random in your graveyard"];
}
