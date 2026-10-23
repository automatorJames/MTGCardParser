namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Raph & Mikey, Troublemakers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raph &amp; Mikey, Troublemakers</exampledoc>
[Dependent]
public class RaphMikeyTroublemakersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal cards from the top of your library until you reveal a creature card"];
}
