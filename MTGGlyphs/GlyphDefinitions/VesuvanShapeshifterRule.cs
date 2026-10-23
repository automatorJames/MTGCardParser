namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vesuvan Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vesuvan Shapeshifter</exampledoc>
/// <examplecapture>if you do, until this creature is turned face down, it becomes a copy of that creature, except it has "at the beginning of your upkeep, you may turn this creature face down</examplecapture>
[TokenizationOrder(-1)]
public class VesuvanShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, until this creature is turned face down, it becomes a copy of that creature, except it has \"at the beginning of your upkeep, you may turn this creature face down"];
}
