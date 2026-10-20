namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards or spells of some kinds, as something to play: "creature spells", "instant and sorcery spells", "spells".</summary>
/// <exampledoc>Vizier of the Menagerie</exampledoc>
/// <examplecapture>creature spells</examplecapture>
[Dependent]
public class PlayableCards : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), Plural(Alt("spell", "card"))];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>> Kinds { get; set; }
}
