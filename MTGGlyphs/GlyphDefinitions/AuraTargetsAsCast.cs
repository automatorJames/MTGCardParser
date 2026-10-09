namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The aura casting rule, from enchant's reminder text: "target a creature as you cast this" - an aura spell targets what it will enchant.</summary>
/// <exampledoc>Fear</exampledoc>
/// <examplecapture>target a creature as you cast this</examplecapture>
public class AuraTargetsAsCast : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Enchanted), "as you cast this"];

    public IndefinitePermanent Enchanted { get; set; }
}
