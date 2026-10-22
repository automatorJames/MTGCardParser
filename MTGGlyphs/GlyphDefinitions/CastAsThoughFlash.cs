namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permission to cast spells at instant speed: "cast this spell as though it had flash", "cast creature spells as though they had flash", "cast colorless spells and artifact spells as though they had flash", "cast spells this turn as though they had flash", "cast this spell as though it had flash if you pay {2} more to cast it". A predicate, after "you may".</summary>
/// <exampledoc>Armor of Thorns</exampledoc>
/// <examplecapture>cast this spell as though it had flash</examplecapture>
[Dependent]
public class CastAsThoughFlash : Glyph, IPredicate
{
    public override Nib[] Nibs => ["cast", Prop(Spells), Prop(Period), "as though", Alt("it had", "they had"), "flash", Prop(ExtraCost)];

    public OneOf<ThisSpell, SpellsCast, ManyOf<SpellsCast>> Spells { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
    [Optional]
    public PayMoreToCast ExtraCost { get; set; }
}
