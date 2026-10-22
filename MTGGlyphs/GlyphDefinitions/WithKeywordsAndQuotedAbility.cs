namespace MTGGlyphs.GlyphDefinitions;

/// <summary>`with flying and "this token can't block`, `with toxic 1 and "this token can't block`, `with flying, trample, and "whenever …`: an object described as having keywords (or one keyword with an amount) and then a quoted ability. The closing quote falls after the period.</summary>
/// <exampledoc>Dragon Egg</exampledoc>
/// <examplecapture>with flying and "{r}: this token gets +1/+0 until end of turn</examplecapture>
[Dependent]
public class WithKeywordsAndQuotedAbility : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["with ", Prop(Keywords), Pattern(",? and \""), Prop(Ability)];

    public OneOf<CompoundOf<Keyword>, KeywordWithAmount> Keywords { get; set; }
    public DynamicGlyph Ability { get; set; }
}
