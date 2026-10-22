namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder for an optional additional cost that isn't mana (kicker or buyback with an action cost, or a mana and an action cost together): "you may sacrifice a land in addition to any other costs as you cast this spell", "you may pay {2}{r} and sacrifice a land in addition to …".</summary>
/// <exampledoc>Falling Timber</exampledoc>
/// <examplecapture>you may sacrifice a land in addition to any other costs as you cast this spell</examplecapture>
public class OptionalActionCostRule : Glyph
{
    public override Nib[] Nibs => ["you may", Prop(Costs), "in addition to any other costs as you cast this spell"];

    public OneOf<ManyOf<Cost>, Cost> Costs { get; set; }
}
