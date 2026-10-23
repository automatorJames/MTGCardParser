namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Retether, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retether</exampledoc>
[Dependent]
public class RetetherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["aura cards that can't enchant a creature on the battlefield remain in your graveyard"];
}
