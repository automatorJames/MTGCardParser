namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target spell or permanent", "target spell or creature", "target spell or permanent with mana value 1 or greater": a target that may be on the stack or on the battlefield.</summary>
/// <exampledoc>Chaoslace</exampledoc>
/// <examplecapture>target spell or permanent</examplecapture>
[Dependent]
public class TargetSpellOrPermanent : Glyph
{
    public override Nib[] Nibs => ["target spell or", Prop(Kind), Prop(With)];

    [Referent]
    public CardType Kind { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
