namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect that happens unless a game state holds, the condition trailing: "sacrifice it unless {u} was spent to cast it", "you lose 2 life unless you control another pirate". Tried after other glyphs, so a trigger keeps its frame and holds this as its effect.</summary>
/// <exampledoc>Azorius Herald</exampledoc>
/// <examplecapture>sacrifice it unless {u} was spent to cast it</examplecapture>
[TokenizationOrder(-1)]
public class EffectUnlessCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "unless", Prop(Condition)];

    public DynamicGlyph Effect { get; set; }
    public OneOf<ManaWasSpent, PlayerControls> Condition { get; set; }
}
