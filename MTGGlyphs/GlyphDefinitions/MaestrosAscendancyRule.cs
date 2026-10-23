namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Maestros Ascendancy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maestros Ascendancy</exampledoc>
/// <examplecapture>once during each of your turns, you may cast an instant or sorcery spell from your graveyard by sacrificing a creature in addition to paying its other costs</examplecapture>
[TokenizationOrder(-1)]
public class MaestrosAscendancyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast an instant or sorcery spell from your graveyard by sacrificing a creature in addition to paying its other costs"];
}
