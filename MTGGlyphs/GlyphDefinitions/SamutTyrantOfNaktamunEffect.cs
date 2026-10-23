namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Samut, Tyrant of Naktamun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samut, Tyrant of Naktamun</exampledoc>
[Dependent]
public class SamutTyrantOfNaktamunEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as a spell with split second is on the stack, players can't cast spells or activate abilities that aren't mana abilities"];
}
