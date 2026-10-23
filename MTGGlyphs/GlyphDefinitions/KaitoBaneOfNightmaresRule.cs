namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kaito, Bane of Nightmares, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaito, Bane of Nightmares</exampledoc>
[TokenizationOrder(-1)]
public class KaitoBaneOfNightmaresRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, as long as kaito has one or more loyalty counters on him, he's a 3/4 ninja creature and has hexproof"];
}
