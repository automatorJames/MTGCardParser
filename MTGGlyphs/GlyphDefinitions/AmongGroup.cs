namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"among creatures you control", "among creatures on the battlefield", "among creatures and planeswalkers they control": the group a superlative is taken over - one plural kind or a list of them, and who controls it, or the whole battlefield. (Not Permanents: that would cycle through WithQualifier.)</summary>
/// <exampledoc>Topple</exampledoc>
/// <examplecapture>among creatures on the battlefield</examplecapture>
[Dependent]
public class AmongGroup : Glyph
{
    public override Nib[] Nibs => ["among", Prop(Kind), Prop(Scope)];

    public OneOf<ManyOf<PluralKind>, PluralKind> Kind { get; set; }
    public OneOf<ControlledBy, BattlefieldScope?> Scope { get; set; }
}
