namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Champion of the Weird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Champion of the Weird</exampledoc>
[Dependent]
public class ChampionOfTheWeirdEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent blights 2"];
}
