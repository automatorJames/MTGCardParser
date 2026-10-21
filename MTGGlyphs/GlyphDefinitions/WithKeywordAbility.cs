namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword ability an object is given as it's made: the "with ward {2}" of a cloaked 2/2.</summary>
/// <exampledoc>Cryptic Coat</exampledoc>
/// <examplecapture>with ward {2}</examplecapture>
[Dependent]
public class WithKeywordAbility : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Ability)];

    public KeywordAbility Ability { get; set; }
}
