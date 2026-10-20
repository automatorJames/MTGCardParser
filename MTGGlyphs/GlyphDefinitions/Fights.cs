namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One creature fighting another: "(target creature you control) fights target creature you don't control", "fights another target creature", "fights that creature".</summary>
/// <exampledoc>Prey Upon</exampledoc>
/// <examplecapture>fights target creature you don't control</examplecapture>
[Dependent]
public class Fights : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("fights?"), Prop(Opponent)];

    public PermanentPhrase Opponent { get; set; }
}
