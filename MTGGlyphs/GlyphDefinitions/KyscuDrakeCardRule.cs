namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kyscu Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kyscu Drake</exampledoc>
[TokenizationOrder(-1)]
public class KyscuDrakeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this creature and a creature named spitting drake: search your library for a card named viashivan dragon, put that card onto the battlefield, then shuffle"];
}
