namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A rule capping how many creatures may attack or block each combat: "no more than one creature can attack each combat", "no more than two creatures can block each combat", "no more than two creatures can attack you each combat", "no more than one creature can attack {this} each combat".</summary>
/// <exampledoc>Dueling Grounds</exampledoc>
/// <examplecapture>no more than one creature can attack each combat</examplecapture>
public class CombatantLimit : Glyph
{
    public override Nib[] Nibs => ["no more than", Prop(Count), Plural("creature"), "can", Prop(Action), Prop(Attacked), "each combat"];

    public Quantity Count { get; set; }
    public CombatAction Action { get; set; }
    [Optional]
    public OneOf<This, AttackRecipient?> Attacked { get; set; }
}
