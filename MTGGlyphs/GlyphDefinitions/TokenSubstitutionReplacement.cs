namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement effect that changes which tokens are created: "if one or more creature tokens would be created under your control, those tokens plus a 1/1 white soldier creature token are created instead", "if one or more tokens would be created under your control, those tokens plus a clue token are created instead", "if one or more creature tokens would be created under your control, that many 4/4 white angel creature tokens with flying and vigilance are created instead". Beside <see cref="TokenIncreaseReplacement"/>, which multiplies the same tokens.</summary>
/// <exampledoc>Divine Visitation</exampledoc>
/// <examplecapture>if one or more creature tokens would be created under your control, that many 4/4 white angel creature tokens with flying and vigilance are created instead</examplecapture>
public class TokenSubstitutionReplacement : Glyph
{
    public override Nib[] Nibs => ["if one or more", Prop(Kind), "tokens would be created", Prop(Control), ",", Prop(Created), "are created instead"];

    [Optional]
    public CardType? Kind { get; set; }
    public UnderControlOf Control { get; set; }
    public OneOf<TokensPlusExtra, TokenDescription> Created { get; set; }
}
