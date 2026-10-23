namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flowstone Sculpture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flowstone Sculpture</exampledoc>
[Dependent]
public class FlowstoneSculptureEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature or this creature gains flying, first strike, or trample"];
}
