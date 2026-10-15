namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Buyback's reminder text.</summary>
/// <exampledoc>Capsize</exampledoc>
/// <examplecapture>you may pay an additional {3} as you cast this spell</examplecapture>
public class BuybackRule : Glyph
{
    public override Nib[] Nibs => ["you may pay an additional", Prop(Cost), "as you cast this spell"];

    public ManaCost Cost { get; set; }
}
