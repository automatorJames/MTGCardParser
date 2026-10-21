namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose abilities: "of artifacts", "of an elemental", "of colorless eldrazi".</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>of artifacts</examplecapture>
[Dependent]
public class AbilitiesOf : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Source)];

    public OneOf<Permanents, IndefinitePermanent> Source { get; set; }
}
