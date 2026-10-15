namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Creating tokens: "create a 1/1 green saproling creature token", or as what a subject does, "(target opponent) creates …".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>create a 1/1 green saproling creature token</examplecapture>
public class CreateToken : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("creates?"), Prop(Token)];

    public TokenDescription Token { get; set; }
}
