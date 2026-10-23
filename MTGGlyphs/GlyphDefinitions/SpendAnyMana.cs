namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"mana of any type can be spent to cast that spell", "you may spend mana as though it were mana of any color to cast it", "… to cast spells this way": a rider letting a spell be paid for with mana of any color or type.</summary>
/// <exampledoc>Gonti, Lord of Luxury</exampledoc>
/// <examplecapture>mana of any type can be spent to cast that spell</examplecapture>
[Dependent]
public class SpendAnyMana : Glyph
{
    public override Nib[] Nibs => [Prop(Permission), "to cast", Prop(Spell)];

    public OneOf<ManaCanBeSpent, MaySpendManaAsThough> Permission { get; set; }
    public OneOf<ThatStackObject, CastObject, SpellsCastThisWay> Spell { get; set; }
}
