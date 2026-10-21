namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Haste's reminder predicate: "can attack and {t} as soon as it comes under your control", "can attack and {t} this turn", "can attack and {t} no matter when it came under your control".</summary>
/// <exampledoc>Arc Runner</exampledoc>
/// <examplecapture>can attack and {t} as soon as it comes under your control</examplecapture>
[Dependent]
public class AttackAndTapAtOnce : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can attack and {t}", Prop(Timing)];

    public HasteTiming Timing { get; set; }
}
