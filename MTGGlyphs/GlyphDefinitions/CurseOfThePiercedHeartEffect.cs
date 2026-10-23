namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curse of the Pierced Heart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of the Pierced Heart</exampledoc>
[Dependent]
public class CurseOfThePiercedHeartEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this aura deals 1 damage to that player or a planeswalker that player controls"];
}
