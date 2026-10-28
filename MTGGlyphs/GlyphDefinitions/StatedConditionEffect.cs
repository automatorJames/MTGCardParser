namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A conditional effect whose condition is a known game state or casting condition: "if it was kicked, search your library for …, reveal it, put it into your hand, then shuffle", "if an opponent controls more lands than you, you may search …", "if {g}{g} was spent to cast it, search …", "if the gift was promised, …", "if this spell's additional cost was paid, …". Exists beside ConditionalEffect because that glyph's open condition slot runs past the first comma when the effect itself has commas; here the condition is typed, so the effect can be a multi-comma sequence. Tried after other glyphs.</summary>
/// <exampledoc>Sprouting Goblin</exampledoc>
/// <examplecapture>if it was kicked, search your library for a land card with a basic land type, reveal it, put it into your hand, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class StatedConditionEffect : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Condition), ",", Prop(Effect)];

    public OneOf<GameState, WasKicked, ControlsPermanent, CardIsKind, ManaWasSpent, KeywordCostWasPaid, CastingChoiceMade> Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
