namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"1 plus the sacrificed creature's mana value": a number added to the sacrificed permanent's characteristic, as a cycle-free bound inside with-qualifiers (Birthing Pod and its kin).</summary>
/// <exampledoc>Birthing Pod</exampledoc>
/// <examplecapture>1 plus the sacrificed creature's mana value</examplecapture>
[Dependent]
public class SacrificedCharacteristicPlus : Glyph
{
    public override Nib[] Nibs => [Prop(Addend), "plus", Prop(Object), "'s", Prop(Characteristic)];

    public Quantity Addend { get; set; }
    public SacrificedPermanent Object { get; set; }
    public Characteristic Characteristic { get; set; }
}
