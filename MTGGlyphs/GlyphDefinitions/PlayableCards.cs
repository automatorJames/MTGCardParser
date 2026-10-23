namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards or spells of some kinds, as something to play: "creature spells", "instant and sorcery spells", "dragon creature spells", "instant and/or sorcery spells", "spells", "spells with mana value 4 or greater", "creature spells of the chosen type". Kinds use the shared space/comma-joined qualifier run.</summary>
/// <exampledoc>Vizier of the Menagerie</exampledoc>
/// <examplecapture>creature spells</examplecapture>
[Dependent]
public class PlayableCards : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), Plural(Alt("spell", "card")), Prop(With)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<QualifierWords>> Kinds { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> With { get; set; }
}
