namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect that happens unless a game state holds, the condition trailing: "sacrifice it unless {u} was spent to cast it", "you lose 2 life unless you control another pirate", "you lose 4 life unless you attacked this turn", "you lose 4 life unless there are four or more card types among cards in your graveyard". Tried after other glyphs, so a trigger keeps its frame and holds this as its effect.</summary>
/// <exampledoc>Azorius Herald</exampledoc>
/// <examplecapture>sacrifice it unless {u} was spent to cast it</examplecapture>
[TokenizationOrder(-1)]
public class EffectUnlessCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "unless", Prop(Condition)];

    public DynamicGlyph Effect { get; set; }
    public OneOf<ManaWasSpent, PlayerControls, GameState> Condition { get; set; }
}
