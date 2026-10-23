namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grimoire of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grimoire of the Dead</exampledoc>
[TokenizationOrder(-1)]
public class GrimoireOfTheDeadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they're black zombies in addition to their other colors and types"];
}
