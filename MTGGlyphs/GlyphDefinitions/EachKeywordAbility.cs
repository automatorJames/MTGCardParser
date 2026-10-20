namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each exhaust ability", "each power-up ability": every ability of a keyworded kind on the object.</summary>
/// <exampledoc>Guidelight Pathmaker</exampledoc>
/// <examplecapture>each exhaust ability</examplecapture>
[Dependent]
public class EachKeywordAbility : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Kind), "ability"];

    public AbilityKind Kind { get; set; }
}
