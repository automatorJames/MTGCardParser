namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sab-Sunen, Luxa Embodied, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sab-Sunen, Luxa Embodied</exampledoc>
[TokenizationOrder(-1)]
public class SabSunenLuxaEmbodiedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sab-sunen can't attack or block unless it has an even number of counters on it"];
}
