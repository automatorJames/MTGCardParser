namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Avacyn's Judgment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Avacyn's Judgment</exampledoc>
[TokenizationOrder(-1)]
public class AvacynsJudgmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell's madness cost was paid, it deals x damage divided as you choose among those permanents and/or players instead"];
}
