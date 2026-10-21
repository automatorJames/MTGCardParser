namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Champion: "champion a goblin or shaman" - the kind of permanent the creature exiles when it enters.</summary>
/// <exampledoc>Lightning Crafter</exampledoc>
/// <examplecapture>champion a goblin or shaman</examplecapture>
[Dependent]
public class ChampionKeyword : Glyph
{
    public override Nib[] Nibs => ["champion", Pattern("an?"), Prop(Kind)];

    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
}
