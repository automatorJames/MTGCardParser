namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A predefined token entering with counters and a quoted ability: `an incubator token with three +1/+1 counters on it and "{2}: transform this token`.</summary>
/// <exampledoc>Sculpted Perfection</exampledoc>
/// <examplecapture>an incubator token with three +1/+1 counters on it and "{2}: transform this token</examplecapture>
[Dependent]
public class PredefinedTokenWithCounters : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Token), " ", Prop(Counters), " and \"", Prop(Ability)];

    public PredefinedTokens Token { get; set; }
    public EntersWithCounters Counters { get; set; }
    public DynamicGlyph Ability { get; set; }
}
