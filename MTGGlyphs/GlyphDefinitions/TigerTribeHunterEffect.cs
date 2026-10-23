namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tiger-Tribe Hunter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tiger-Tribe Hunter</exampledoc>
[Dependent]
public class TigerTribeHunterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, you may sacrifice another creature"];
}
