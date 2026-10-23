namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Meld's condition: "you both own and control this creature and a creature named midnight scavengers", "you both own and control gisela and a creature named bruna, the fading light" - this card (or its short name) and its named meld partner.</summary>
/// <exampledoc>Graf Rats // Chittering Host</exampledoc>
/// <examplecapture>you both own and control this creature and a creature named midnight scavengers</examplecapture>
[Dependent]
public class OwnAndControlBoth : Glyph
{
    public override Nib[] Nibs => ["you both own and control", Prop(Self), Opt(Pattern(@"[a-z][a-z'\-]*(?:,? [a-z][a-z'\-]*)*?")), "and", Pattern("an?"), Prop(PartnerKind), Prop(Partner)];

    [Optional]
    public ThisCreature Self { get; set; }
    public CardType PartnerKind { get; set; }
    public NamedCard Partner { get; set; }
}
