namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Not having moved a looked-at card somewhere, as a condition for a fallback: "you don't put the card into your hand", "you didn't put a card onto the battlefield this way".</summary>
/// <exampledoc>Traveling Botanist</exampledoc>
/// <examplecapture>you don't put the card into your hand</examplecapture>
[Dependent]
public class DidntPutCardCondition : Glyph
{
    public override Nib[] Nibs => ["you", Pattern("(don't|didn't)"), "put", Pattern("(the|a) card"), Prop(Destination), Prop(ThisWay)];

    public OneOf<IntoZone, ToTheBattlefieldUnderControl> Destination { get; set; }
    [RegexPattern("this way")]
    public bool ThisWay { get; set; }
}
