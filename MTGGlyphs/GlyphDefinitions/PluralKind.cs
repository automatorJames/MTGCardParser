namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"island", "zombies", "artifacts": a permanent kind (card, land or creature type), singular or plural.</summary>
/// <exampledoc>Lingering Mirage</exampledoc>
/// <examplecapture>island</examplecapture>
[Dependent]
public class PluralKind : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind))];

    public PermanentKind Kind { get; set; }
}
