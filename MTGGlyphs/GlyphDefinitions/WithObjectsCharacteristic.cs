namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with that spell's mana value": a characteristic equal to that of a spell referred to earlier, as on the Kirins' token makers.</summary>
/// <exampledoc>Kura, the Boundless Sky</exampledoc>
/// <examplecapture>with that spell's mana value</examplecapture>
[Dependent]
public class WithObjectsCharacteristic : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Object), "'s", Prop(Characteristic)];

    public SpellReference Object { get; set; }
    public Characteristic Characteristic { get; set; }
}
