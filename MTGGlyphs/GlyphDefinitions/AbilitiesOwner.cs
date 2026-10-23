namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of planeswalkers your opponents control", "of other permanents you control": whose abilities are meant.</summary>
/// <exampledoc>Eidolon of Obstruction</exampledoc>
/// <examplecapture>of planeswalkers your opponents control</examplecapture>
[Dependent]
public class AbilitiesOwner : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Permanents)];

    public OneOf<OtherPermanents, Permanents> Permanents { get; set; }
}
