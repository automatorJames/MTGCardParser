namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect that happens unless a cost is paid: "sacrifice {this} unless you pay {u}", "{this} deals 8 damage to you unless you pay {g}{g}{g}{g}". Tried after other glyphs, so a trigger keeps its frame and holds this as its effect.</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>sacrifice {this} unless you pay {u}</examplecapture>
[TokenizationOrder(-1)]
public class UnlessPaid : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), Prop(Payment)];

    public DynamicGlyph Effect { get; set; }
    public OptionalPayCost Payment { get; set; }
}
