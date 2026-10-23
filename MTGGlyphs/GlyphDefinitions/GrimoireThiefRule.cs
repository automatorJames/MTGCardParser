namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grimoire Thief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grimoire Thief</exampledoc>
[TokenizationOrder(-1)]
public class GrimoireThiefRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter all spells with those names"];
}
