namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Creating tokens: "create a 1/1 green saproling creature token", "create a treasure token", "create a token that's a copy of target creature you control", "create a 1/1 black rat creature token with "this token can't block"", "create a 1/1 colorless sand creature token for each land you control", "create a number of 1/1 black harpy creature tokens with flying equal to your devotion to black", "create an incubator token with three +1/+1 counters on it and "{2}: transform this token"", or as what a subject does, "(target opponent) creates …".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>create a 1/1 green saproling creature token</examplecapture>
public class CreateToken : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("creates?"), Prop(Token), Prop(PerEach), Prop(EqualTo)];

    public OneOf<TokenWithQuotedAbility, TokenDescription, OneOf<PredefinedTokenWithCounters, PredefinedTokens, TokenCopy>> Token { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
    [Optional]
    public EqualTo EqualTo { get; set; }
}
