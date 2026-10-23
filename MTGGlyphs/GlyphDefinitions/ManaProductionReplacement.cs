namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement multiplying the mana a permanent produces: "if you tap a permanent for mana, it produces twice as much of that mana instead", "if you tap a basic land for mana, it produces three times as much of that mana instead". The replacement counterpart of <see cref="TapForManaTrigger"/>.</summary>
/// <exampledoc>Mana Reflection</exampledoc>
/// <examplecapture>if you tap a permanent for mana, it produces twice as much of that mana instead</examplecapture>
public class ManaProductionReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Player), Alt("tap", "taps"), Prop(Permanent), "for mana, it produces", Prop(Factor), "as much of that mana instead"];

    public WhichPlayer Player { get; set; }
    public IndefinitePermanent Permanent { get; set; }
    public ScaleFactor Factor { get; set; }
}
