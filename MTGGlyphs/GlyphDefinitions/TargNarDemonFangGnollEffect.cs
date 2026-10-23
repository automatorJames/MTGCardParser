namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Targ Nar, Demon-Fang Gnoll, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Targ Nar, Demon-Fang Gnoll</exampledoc>
/// <examplecapture>if you attacked with creatures with total power 6 or greater this combat, attacking creatures get +1/+0 until end of turn</examplecapture>
[Dependent]
public class TargNarDemonFangGnollEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, attacking creatures get +1/+0 until end of turn"];
}
