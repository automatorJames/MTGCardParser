namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"can't be regenerated", "can't be regenerated this turn": a predicate, so its subject can be "it", "they" (what was just destroyed) or a target.</summary>
/// <exampledoc>Hurr Jackal</exampledoc>
/// <examplecapture>can't be regenerated this turn</examplecapture>
[Dependent]
public class CantBeRegenerated : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't be regenerated", Prop(Duration)];

    [Optional]
    public ThisPeriod Duration { get; set; }
}
