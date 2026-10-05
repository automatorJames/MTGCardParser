namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"[cost]: [effect]", e.g. "{2}, {t}: draw a card".</summary>
public class ActivatedAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Costs), ":", Prop(Effect)];

    public ActivationCosts Costs { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}

/// <summary>An activated ability's costs, comma-joined: "{2}, {t}".</summary>
[Dependent]
public class ActivationCosts : CompoundOf<ActivationCost>;

[Dependent]
public class ActivationCost : GlyphOneOf
{
    public ManaValue ManaValue { get; set; }
    public TapSymbol? TapSymbol { get; set; }
}

public enum TapSymbol
{
    [RegexPattern(@"\{t}")] Tap,
    [RegexPattern(@"\{q}")] Untap,
}
