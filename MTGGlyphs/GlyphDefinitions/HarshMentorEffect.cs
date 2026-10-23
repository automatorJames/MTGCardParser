namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Harsh Mentor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harsh Mentor</exampledoc>
/// <examplecapture>creature, or land on the battlefield, if it isn't a mana ability, this creature deals 2 damage to that player</examplecapture>
[Dependent]
public class HarshMentorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature, or land on the battlefield, if it isn't a mana ability, this creature deals 2 damage to that player"];
}
