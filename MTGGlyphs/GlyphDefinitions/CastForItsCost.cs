namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost's reminder: "you may cast this spell for its overload cost", "… for its evoke cost", "… for its dash cost", "you may cast this spell for its surge cost if you or a teammate has cast another spell this turn", "you may cast this spell for its spectacle cost rather than its mana cost if an opponent lost life this turn". The keyword is captured; a trailing condition is optional. Replaces the per-keyword EvokeCastRule and DashRule.</summary>
/// <exampledoc>Cyclonic Rift</exampledoc>
/// <examplecapture>you may cast this spell for its overload cost</examplecapture>
public class CastForItsCost : Glyph
{
    public override Nib[] Nibs => ["you may cast", Alt("this spell", "this card", "this"), "for its", Prop(Keyword), "cost", Prop(RatherThanMana), Prop(Condition)];

    public Keyword Keyword { get; set; }
    [RegexPattern("rather than its mana cost")]
    public bool RatherThanMana { get; set; }
    [Optional]
    public AltCostCondition Condition { get; set; }
}
