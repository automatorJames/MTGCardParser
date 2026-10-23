namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Champion of Dusan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Champion of Dusan</exampledoc>
[Dependent]
public class ChampionOfDusanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and a trample counter on target creature"];
}
