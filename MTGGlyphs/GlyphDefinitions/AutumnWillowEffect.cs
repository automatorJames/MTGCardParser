namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Autumn Willow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Autumn Willow</exampledoc>
[Dependent]
public class AutumnWillowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn,", Nib.This, "can be the target of spells and abilities controlled by target player as though it didn't have shroud"];
}
