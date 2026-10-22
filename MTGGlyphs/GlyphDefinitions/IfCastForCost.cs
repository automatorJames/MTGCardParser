namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost's rider: what else happens if the spell was cast for that cost - "if you cast this spell for {4}{u}, also put three +1/+1 counters on target land you control and it becomes a 0/0 elemental creature with haste" (awaken), "if you cast this spell for its blitz cost, …". The cost is a mana cost or "its [keyword] cost".</summary>
/// <exampledoc>Clutch of Currents</exampledoc>
/// <examplecapture>if you cast this spell for {4}{u}, also put three +1/+1 counters on target land you control and it becomes a 0/0 elemental creature with haste</examplecapture>
public class IfCastForCost : Glyph
{
    public override Nib[] Nibs => ["if you cast this spell for", Prop(Cost), ",", Prop(Also), Prop(Effect)];

    public OneOf<ManaCost, ItsKeywordCost> Cost { get; set; }
    [RegexPattern("also")]
    public bool Also { get; set; }
    public DynamicGlyph Effect { get; set; }
}
