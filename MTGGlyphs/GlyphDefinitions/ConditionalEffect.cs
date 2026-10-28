namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A conditional effect: "if you control an urza's mine and an urza's tower, add {c}{c}{c}", "if it was kicked, search your library for …, reveal it, put it into your hand, then shuffle", and what follows the outcome of a choice, a requirement or a coin flip: "if you do, …", "if the player doesn't, …", "if you win the flip, …". A replacement effect is the same frame, with "instead" inside the effect: "if you would gain life, you gain that much life plus 1 instead" (<see cref="EffectThenInstead"/>), "if you would draw a card, instead reveal the top three cards of your library" (<see cref="InsteadThenEffect"/>). The condition is typed where it can be - an outcome, a game state, a casting condition - which bounds it at the first comma, so the effect can be a multi-comma sequence; anything else takes the text up to the last comma before the effect. Tried after other glyphs, so the "instead" and replacement frames keep their lines.</summary>
/// <exampledoc>Urza's Tower</exampledoc>
/// <examplecapture>if you control an urza's mine and an urza's power-plant, add {c}{c}{c}</examplecapture>
[TokenizationOrder(-1)]
public class ConditionalEffect : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Condition), ",", Prop(Effect)];

    public OneOf<Outcome, CardIsKind, GameState, ControlsPermanent, WasKicked, ManaWasSpent, KeywordCostWasPaid, CastingChoiceMade, DynamicGlyph> Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
