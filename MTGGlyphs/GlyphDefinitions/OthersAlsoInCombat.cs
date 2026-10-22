namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Other creatures attacking or blocking alongside, as a condition on a combat restriction: "a creature with greater power also attacks", "at least two other creatures block", "a black or green creature also attacks".</summary>
/// <exampledoc>Okk</exampledoc>
/// <examplecapture>a creature with greater power also attacks</examplecapture>
[Dependent]
public class OthersAlsoInCombat : Glyph
{
    public override Nib[] Nibs => [Prop(Who), Opt("also"), Plural(Prop(Action))];

    public OneOf<IndefinitePermanent, AtLeastPermanents> Who { get; set; }
    public CombatAction Action { get; set; }
}
