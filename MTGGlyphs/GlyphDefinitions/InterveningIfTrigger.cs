namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability with an intervening "if" clause, whose trigger condition isn't modelled by a more specific trigger glyph: "when an opponent casts an enchantment spell, if this permanent is an enchantment, it becomes a 5/5 treefolk creature". The sibling of <see cref="TriggeredAbility"/>, which can't hold a second comma-separated clause. Tried after the specific triggers.</summary>
/// <exampledoc>Hidden Ancients</exampledoc>
/// <examplecapture>when an opponent casts an enchantment spell, if this permanent is an enchantment, it becomes a 5/5 treefolk creature</examplecapture>
[TokenizationOrder(-1)]
public class InterveningIfTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Event), ", if", Prop(Condition), ",", Prop(Effect)];

    public DynamicGlyph Event { get; set; }
    public DynamicGlyph Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
