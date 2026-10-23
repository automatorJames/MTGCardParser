namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nighthowler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nighthowler</exampledoc>
[TokenizationOrder(-1)]
public class NighthowlerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature and enchanted creature each get +x/+x, where x is the number of creature cards in all graveyards"];
}
