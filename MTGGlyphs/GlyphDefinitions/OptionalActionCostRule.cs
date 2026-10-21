namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder for an optional additional cost that isn't mana (kicker or buyback with an action cost): "you may sacrifice a land in addition to any other costs as you cast this spell".</summary>
/// <exampledoc>Falling Timber</exampledoc>
/// <examplecapture>you may sacrifice a land in addition to any other costs as you cast this spell</examplecapture>
public class OptionalActionCostRule : Glyph
{
    public override Nib[] Nibs => ["you may", Prop(Cost), "in addition to any other costs as you cast this spell"];

    public Cost Cost { get; set; }
}
