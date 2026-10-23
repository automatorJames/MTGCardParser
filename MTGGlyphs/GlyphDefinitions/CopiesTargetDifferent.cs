namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rule for spells copied once per other legal target: "each copy targets a different one of those creatures", "… of those permanents and players".</summary>
/// <exampledoc>Precursor Golem</exampledoc>
/// <examplecapture>each copy targets a different one of those golems</examplecapture>
public class CopiesTargetDifferent : Glyph
{
    public override Nib[] Nibs => ["each copy targets a different one of those", Prop(Kinds)];

    public OneOf<ManyOf<PluralTargetKind>, PluralTargetKind> Kinds { get; set; }
}
