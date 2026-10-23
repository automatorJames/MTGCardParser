namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"for artifacts and creatures", "for an enchantment and a planeswalker", "for flying, first strike, … and vigilance": what a repeated process is done again with, in place of the original.</summary>
/// <exampledoc>Equipoise</exampledoc>
/// <examplecapture>for artifacts and creatures</examplecapture>
[Dependent]
public class RepeatFor : Glyph
{
    public override Nib[] Nibs => ["for", Prop(Items)];

    public OneOf<ManyOf<IndefinitePermanent>, ManyOf<PluralKind>, ManyOf<Keyword?>> Items { get; set; }
}
