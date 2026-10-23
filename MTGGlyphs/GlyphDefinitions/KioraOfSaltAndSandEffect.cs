namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kiora of Salt and Sand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kiora of Salt and Sand</exampledoc>
[Dependent]
public class KioraOfSaltAndSandEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've activated a loyalty ability this turn, untap target attacking creature"];
}
