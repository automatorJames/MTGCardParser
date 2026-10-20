namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability that applies only while a condition holds, the condition first: "as long as there are seven or more cards in your graveyard, this creature gets +1/+1 and has flying", "as long as you control three or more artifacts, this creature gets +2/+2", "as long as this creature has a +1/+1 counter on it, …".</summary>
/// <exampledoc>Mystic Crusader</exampledoc>
/// <examplecapture>as long as there are seven or more cards in your graveyard, this creature gets +1/+1 and has flying</examplecapture>
public class WhileConditionStatic : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(Condition), ",", Prop(Effect)];

    public OneOf<ItsAspect, HasCounters, OneOf<CardCountCondition, PlayerControls>> Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
