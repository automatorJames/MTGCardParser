namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount given as "equal to" a count or a characteristic: "equal to that creature's toughness", "equal to the number of swamps they control".</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>equal to the number of swamps they control</examplecapture>
[Dependent]
public class EqualTo : Glyph
{
    public override Nib[] Nibs => ["equal to", Prop(Measure)];

    public OneOf<NumberOf, CharacteristicOf> Measure { get; set; }
}
