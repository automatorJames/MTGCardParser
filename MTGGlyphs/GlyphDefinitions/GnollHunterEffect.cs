namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gnoll Hunter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gnoll Hunter</exampledoc>
/// <examplecapture>if you attacked with creatures with total power 6 or greater this combat, put a +1/+1 counter on this creature</examplecapture>
[Dependent]
public class GnollHunterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, put a +1/+1 counter on this creature"];
}
