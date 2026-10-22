namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards outside the battlefield an effect on permanents extends to: "creature spells you control and creature cards you own that aren't on the battlefield", "spells you control and nonland cards you own that aren't on the battlefield".</summary>
/// <exampledoc>Conspiracy</exampledoc>
/// <examplecapture>creature spells you control and creature cards you own that aren't on the battlefield</examplecapture>
[Dependent]
public class SpellsAndCardsOffBattlefield : Glyph
{
    public override Nib[] Nibs => [Prop(SpellQualifiers), "spells you control and", Prop(CardQualifiers), "cards you own that aren't on the battlefield"];

    [Optional]
    public QualifierWords SpellQualifiers { get; set; }
    [Optional]
    public QualifierWords CardQualifiers { get; set; }
}
