namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Whim of Volrath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whim of Volrath</exampledoc>
[Dependent]
public class WhimOfVolrathEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, you may change \"nonred creature\" to \"nongreen creature\" or \"plainswalk\" to \"swampwalk"];
}
