namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability that applies only while a condition holds, the condition first: "as long as there are seven or more cards in your graveyard, this creature gets +1/+1 and has flying", "as long as you control three or more artifacts, this creature gets +2/+2", "as long as the top card of your library is a creature card, …", "as long as this creature is on the battlefield, …". The condition may join two states with "and".</summary>
/// <exampledoc>Mystic Crusader</exampledoc>
/// <examplecapture>as long as there are seven or more cards in your graveyard, this creature gets +1/+1 and has flying</examplecapture>
public class WhileConditionStatic : Glyph
{
    public override Nib[] Nibs => ["as long as", Prop(Condition), ",", Prop(Effect)];

    public OneOf<BothStates, GameState, CardIsKind, IsOnBattlefield> Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
