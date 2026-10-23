namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy renamed, retyped and given abilities: "except its name is {this}, it's legendary in addition to its other types, and it has hexproof and this ability", `… and it has "{2}{u}{u}: …"`.</summary>
/// <exampledoc>Lazav, Dimir Mastermind</exampledoc>
/// <examplecapture>except its name is {this}, it's legendary in addition to its other types, and it has hexproof and this ability</examplecapture>
[Dependent]
public class CopyRenamedRetypedGranted : Glyph
{
    public override Nib[] Nibs => ["except", Prop(Whose), "name is", Nib.This, ",", Prop(Pronoun), "'s", Prop(Kind), ", and", Prop(Holder), Prop(Granted)];

    public CopyPossessive Whose { get; set; }
    public CopyPronoun Pronoun { get; set; }
    public CopyKind Kind { get; set; }
    public CopyPronoun Holder { get; set; }
    public OneOf<CopyHasKeywords, GrantsQuotedAbility> Granted { get; set; }
}
