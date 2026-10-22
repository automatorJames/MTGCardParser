namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents of a kind on the battlefield as a condition, written without "there": "no creatures are on the battlefield", "an enchantment is on the battlefield", "no permanents named {this} are on the battlefield". The "there are …" form is PermanentsOnBattlefield.</summary>
/// <exampledoc>Call to the Grave</exampledoc>
/// <examplecapture>no creatures are on the battlefield</examplecapture>
[Dependent]
public class PermanentsAreOnBattlefield : Glyph
{
    public override Nib[] Nibs => [Prop(What), Alt("are", "is"), "on the battlefield"];

    public OneOf<PermanentsThreshold, NoPermanents, IndefinitePermanent> What { get; set; }
}
