namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A token described with a quoted ability of its own: `a 1/1 black rat creature token with "this token can't block`.</summary>
/// <exampledoc>Voracious Vermin</exampledoc>
/// <examplecapture>a 1/1 black rat creature token with "this token can't block</examplecapture>
[Dependent]
public class TokenWithQuotedAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Token), Prop(Quoted)];

    public TokenDescription Token { get; set; }
    public WithQuotedAbility Quoted { get; set; }
}
