namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat permission that ignores a keyword: "can attack as though it didn't have defender", "can attack this turn as though it didn't have defender", "can attack as though it had haste", "can be blocked as though they didn't have forestwalk", "can attack as though they didn't have defender", or which creatures it can block that way ("can block creatures with shadow as though it had shadow", "can block dragons as though it had reach"), possibly only while a condition holds ("… as long as you control a gate"). The pronoun is the subject, or the blocked creatures, read back. Also the second of two actions ("gets +3/-1 until end of turn and can attack this turn as though …").</summary>
/// <exampledoc>Animate Wall</exampledoc>
/// <examplecapture>can attack as though it didn't have defender</examplecapture>
[Dependent]
public class CanAttackAsThough : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can", Prop(Action), Prop(Blocked), Prop(Duration), "as though", Prop(Bearer), Prop(Possession), Prop(Keyword), Prop(While)];

    public CombatAction Action { get; set; }
    [Optional]
    public Permanents Blocked { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    public OneOf<It, They> Bearer { get; set; }
    public Possession Possession { get; set; }
    public Keyword Keyword { get; set; }
    [Optional]
    public EffectDuration While { get; set; }
}
