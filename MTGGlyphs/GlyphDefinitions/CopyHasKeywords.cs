namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Keyword abilities a copy has, perhaps with the copying ability itself: "has hexproof and this ability", "has vigilance", "has this ability".</summary>
/// <exampledoc>Lazav, Dimir Mastermind</exampledoc>
/// <examplecapture>has hexproof and this ability</examplecapture>
[Dependent]
public class CopyHasKeywords : Glyph
{
    public override Nib[] Nibs => ["has", Prop(Keywords), Prop(AndThisAbility)];

    public OneOf<ThisAbility, ManyOf<Keyword>, Keyword?> Keywords { get; set; }
    [RegexPattern("and this ability")]
    public bool AndThisAbility { get; set; }
}
