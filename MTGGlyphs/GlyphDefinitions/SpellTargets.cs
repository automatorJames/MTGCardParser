namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that targets this creature", "that targets a creature", "that targets a permanent you control", "that targets you", "that targets you or a creature you control", "that targets only a single creature", "that targets one or more creatures": a spell aimed at something, optionally saying how many targets it has.</summary>
/// <exampledoc>Heroes of the Revel</exampledoc>
/// <examplecapture>that targets this creature</examplecapture>
[Dependent]
public class SpellTargets : Glyph
{
    public override Nib[] Nibs => ["that targets", Prop(Count), Prop(Targeted)];

    [Optional]
    public SpellTargetCount? Count { get; set; }
    public OneOf<PlayerOrPermanent, PermanentPhrase, Permanents, IndefinitePermanent, WhichPlayer?> Targeted { get; set; }
}
