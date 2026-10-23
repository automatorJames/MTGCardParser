namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Apocalypse Hydra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Apocalypse Hydra</exampledoc>
[TokenizationOrder(-1)]
public class ApocalypseHydraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if x is 5 or more, it enters with an additional x +1/+1 counters on it"];
}
