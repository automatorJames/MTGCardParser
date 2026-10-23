namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Venerable Warsinger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Venerable Warsinger</exampledoc>
/// <examplecapture>you may return target creature card with mana value x or less from your graveyard to the battlefield, where x is the amount of damage this creature dealt to that player</examplecapture>
[Dependent]
public class VenerableWarsingerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return target creature card with mana value x or less from your graveyard to the battlefield, where x is the amount of damage this creature dealt to that player"];
}
