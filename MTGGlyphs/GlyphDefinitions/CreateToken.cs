namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Creating tokens: "create a 1/1 green saproling creature token", "create a treasure token", "create a token that's a copy of target creature you control", "create a 1/1 black rat creature token with "this token can't block"", "create a 1/1 colorless sand creature token for each land you control", "create a number of 1/1 black harpy creature tokens with flying equal to your devotion to black", "create an incubator token with three +1/+1 counters on it and "{2}: transform this token"", "create twelve of those tokens" (more of the tokens an earlier effect created), "create ragavan, a legendary 2/1 red monkey creature token" (a named legendary token), or as what a subject does, "(target opponent) creates …". Also the second choice of an "or" pair, as endure's and fabricate's reminders: "put a +1/+1 counter on it or create a 1/1 white spirit creature token".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>create a 1/1 green saproling creature token</examplecapture>
public class CreateToken : Glyph, IAlternativePredicate, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("creates?"), Prop(Token), Prop(PerEach), Prop(EqualTo)];

    public OneOf<TokenWithQuotedAbility, NamedLegendaryToken, TokenDescription, PredefinedTokenWithCounters, PredefinedTokens, TokenCopy, CountOfThoseTokens> Token { get; set; }
    [Optional]
    public ForEach PerEach { get; set; }
    [Optional]
    public EqualTo EqualTo { get; set; }
}
