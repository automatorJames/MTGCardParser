namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that land's controller": the controller of a permanent named earlier.</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>that land's controller</examplecapture>
[Dependent]
public class ThatCardsController : Glyph
{
    public override Nib[] Nibs => ["that", Prop(CardOrCreatureType), "'s controller"];

    public OneOf<CardType?, CreatureType?> CardOrCreatureType { get; set; }
}
