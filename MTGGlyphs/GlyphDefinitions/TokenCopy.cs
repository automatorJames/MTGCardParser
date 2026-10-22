namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a token that's a copy of target creature you control", "a token that's a copy of it", "x tokens that are copies of …", "a tapped token that's a copy of it", "a tapped and attacking token that's a copy of it", "a 1/1 token copy of it" (offspring), "a token that's a copy of it, except it's a 4/4 black zombie" (eternalize): tokens copying an existing object, optionally entering tapped (and attacking), with set power and toughness and with exceptions to the copied values.</summary>
/// <exampledoc>Cackling Counterpart</exampledoc>
/// <examplecapture>a token that's a copy of target creature you control</examplecapture>
[Dependent]
public class TokenCopy : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Tapped), Prop(Stats), Plural("token"), Alt("that's a copy of", "that are copies of", "copy of"), Prop(Original)];

    public Quantity Quantity { get; set; }
    [Optional]
    public TokenEntryState? Tapped { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    public OneOf<CopiedWithExceptions, PermanentPhrase, IndefinitePermanent> Original { get; set; }
}
