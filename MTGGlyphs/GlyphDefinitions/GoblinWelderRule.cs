namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goblin Welder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Welder</exampledoc>
[TokenizationOrder(-1)]
public class GoblinWelderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if both targets are still legal as this ability resolves, that player simultaneously sacrifices the artifact and returns the artifact card to the battlefield"];
}
