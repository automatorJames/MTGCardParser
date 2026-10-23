namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Littjara Kinseekers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Littjara Kinseekers</exampledoc>
/// <examplecapture>if you control three or more creatures that share a creature type, put a +1/+1 counter on this creature, then scry 1</examplecapture>
[Dependent]
public class LittjaraKinseekersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control three or more creatures that share a creature type, put a +1/+1 counter on this creature, then scry 1"];
}
