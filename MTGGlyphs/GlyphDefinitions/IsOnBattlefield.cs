namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The card itself being on the battlefield, as a condition: "if this creature is on the battlefield", "as long as this artifact is on the battlefield", "if {this} is still on the battlefield". Other permanents on the battlefield are PermanentsAreOnBattlefield.</summary>
/// <exampledoc>Necrotic Ooze</exampledoc>
/// <examplecapture>this creature is on the battlefield</examplecapture>
[Dependent]
public class IsOnBattlefield : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "is", Opt("still"), "on the battlefield"];

    public OneOf<This, ThisCreature> Permanent { get; set; }
}
