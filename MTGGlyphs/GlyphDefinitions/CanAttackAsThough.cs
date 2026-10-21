namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat permission that ignores a keyword: "can attack as though it didn't have defender", "can attack this turn as though it didn't have defender", "can attack as though it had haste", "can be blocked as though they didn't have forestwalk", "can attack as though they didn't have defender". The pronoun is the subject, read back.</summary>
/// <exampledoc>Animate Wall</exampledoc>
/// <examplecapture>can attack as though it didn't have defender</examplecapture>
[Dependent]
public class CanAttackAsThough : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can", Prop(Action), Prop(Duration), "as though", Prop(Bearer), Prop(Possession), Prop(Keyword)];

    public CombatAction Action { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
    public OneOf<It, They> Bearer { get; set; }
    public Possession Possession { get; set; }
    public Keyword Keyword { get; set; }
}
