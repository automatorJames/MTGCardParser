namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent becoming an aura that can enchant something: "becomes an aura enchantment with enchant creature" (Licids), "becomes an aura with enchant creature".</summary>
/// <exampledoc>Calming Licid</exampledoc>
/// <examplecapture>becomes an aura enchantment with enchant creature</examplecapture>
public class BecomesAura : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("becomes?"), "an aura", Prop(Enchantment), "with", Prop(Enchant)];

    [RegexPattern("enchantment")]
    public bool Enchantment { get; set; }
    public EnchantPermanent Enchant { get; set; }
}
