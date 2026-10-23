namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sab-Sunen, Luxa Embodied, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sab-Sunen, Luxa Embodied</exampledoc>
[TokenizationOrder(-1)]
public class SabSunenLuxaEmbodied2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if it has an odd number of counters on it, draw two cards"];
}
