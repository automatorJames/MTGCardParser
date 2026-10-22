namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the total power of creatures you control": a characteristic summed over a group - permanents, or objects named earlier ("the total power of the sacrificed creatures", "the total mana value of those cards", "the total power of the exiled cards").</summary>
/// <exampledoc>Ghalta, Primal Hunger</exampledoc>
/// <examplecapture>the total power of creatures you control</examplecapture>
[Dependent]
public class TotalOf : Glyph
{
    public override Nib[] Nibs => ["the total", Prop(Characteristic), "of", Prop(Group)];

    public Characteristic Characteristic { get; set; }
    public OneOf<Permanents, OneOf<SacrificedPermanent, ThatCardPhrase>, ThatCard> Group { get; set; }
}
