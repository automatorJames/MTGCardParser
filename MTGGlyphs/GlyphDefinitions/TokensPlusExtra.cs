namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"those tokens plus a 1/1 white soldier creature token", "those tokens plus a clue token": the tokens about to be created, and one more besides.</summary>
/// <exampledoc>Queen Allenal of Ruadach</exampledoc>
/// <examplecapture>those tokens plus a 1/1 white soldier creature token</examplecapture>
[Dependent]
public class TokensPlusExtra : Glyph
{
    public override Nib[] Nibs => ["those tokens plus", Prop(Extra)];

    public OneOf<TokenDescription, PredefinedTokens> Extra { get; set; }
}
