namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the sacrificed creature": the permanent sacrificed to pay the ability's cost.</summary>
/// <exampledoc>Diamond Valley</exampledoc>
/// <examplecapture>the sacrificed creature</examplecapture>
[Dependent]
public class SacrificedPermanent : Glyph
{
    public override Nib[] Nibs => ["the sacrificed", Prop(CardType)];

    public CardType CardType { get; set; }
}
