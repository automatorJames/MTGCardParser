namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Devotion's reminder: "each {b} in the mana costs of permanents you control counts toward your devotion to black", "each {w} and {u} … counts toward your devotion to white and blue".</summary>
/// <exampledoc>Marshmist Titan</exampledoc>
/// <examplecapture>each {b} in the mana costs of permanents you control counts toward your devotion to black</examplecapture>
public class DevotionRule : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Symbols), "in the mana costs of permanents you control counts toward", Prop(Devotion)];

    public OneOf<ManyOf<ManaCost>, ManaCost> Symbols { get; set; }
    public DevotionTo Devotion { get; set; }
}
