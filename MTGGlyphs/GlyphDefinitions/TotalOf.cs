namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the total power of creatures you control": a characteristic summed over a group - permanents, or objects named earlier ("the total power of the sacrificed creatures", "the total mana value of those cards", "the total power of the exiled cards"). The frame records "other", excluding this permanent ("the total mana value of other creatures you control").</summary>
/// <exampledoc>Ghalta, Primal Hunger</exampledoc>
/// <examplecapture>the total power of creatures you control</examplecapture>
[Dependent]
public class TotalOf : Glyph
{
    public override Nib[] Nibs => ["the total", Prop(Characteristic), "of", Opt("other"), Prop(Group)];

    public Characteristic Characteristic { get; set; }
    public OneOf<Permanents, HandledObject, ThatCardPhrase, ThatCard> Group { get; set; }
}
