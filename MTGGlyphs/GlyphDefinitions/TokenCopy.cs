namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a token that's a copy of target creature you control", "a token that's a copy of it", "x tokens that are copies of …", "a 1/1 token copy of it" (offspring): tokens copying an existing object, optionally with set power and toughness.</summary>
/// <exampledoc>Cackling Counterpart</exampledoc>
/// <examplecapture>a token that's a copy of target creature you control</examplecapture>
[Dependent]
public class TokenCopy : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Stats), Plural("token"), Alt("that's a copy of", "that are copies of", "copy of"), Prop(Original)];

    public Quantity Quantity { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> Original { get; set; }
}
