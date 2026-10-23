namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Varchild's War-Riders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Varchild's War-Riders</exampledoc>
[TokenizationOrder(-1)]
public class VarchildsWarRidersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—have an opponent create a 1/1 red survivor creature token"];
}
