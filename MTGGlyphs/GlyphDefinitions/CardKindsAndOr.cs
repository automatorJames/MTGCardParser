namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A list of card kinds joined by "and/or" - up to one of each: "an angel card, a demon card, and/or a dragon card", "a creature card and/or a land card".</summary>
/// <exampledoc>Kaalia, Zenith Seeker</exampledoc>
/// <examplecapture>an angel card, a demon card, and/or a dragon card</examplecapture>
[Dependent]
public class CardKindsAndOr : Glyph
{
    public override Nib[] Nibs => [Prop(Others), Pattern(",? and/or"), Prop(Last)];

    public CompoundOf<IndefiniteCardKind> Others { get; set; }
    public IndefiniteCardKind Last { get; set; }
}
