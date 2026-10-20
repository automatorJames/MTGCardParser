namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a "for each" counts among objects: "artifact you control", "other creature you control", "creature card in your graveyard", "card in your hand".</summary>
/// <exampledoc>Mephitic Ooze</exampledoc>
/// <examplecapture>artifact you control</examplecapture>
[Dependent]
public class CountedObjects : Glyph
{
    public override Nib[] Nibs => [Prop(Other), Prop(Objects)];

    [RegexPattern("other")]
    public bool Other { get; set; }
    public OneOf<CardsInZone, Permanents> Objects { get; set; }
}
