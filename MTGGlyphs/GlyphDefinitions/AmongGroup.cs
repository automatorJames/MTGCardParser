namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"among creatures you control", "among creatures on the battlefield": the group a superlative is taken over - a plural kind and who controls it, or the whole battlefield. (Not Permanents: that would cycle through WithQualifier.)</summary>
/// <exampledoc>Topple</exampledoc>
/// <examplecapture>among creatures on the battlefield</examplecapture>
[Dependent]
public class AmongGroup : Glyph
{
    public override Nib[] Nibs => ["among", Prop(Kind), Prop(Scope)];

    public ManyOf<PluralKind> Kind { get; set; }
    public OneOf<ControlledBy, BattlefieldScope?> Scope { get; set; }
}
