namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Evermind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evermind</exampledoc>
[Dependent]
public class EvermindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nonexistent mana costs can't be paid"];
}
