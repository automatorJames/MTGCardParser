namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rivals' Duel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rivals' Duel</exampledoc>
[TokenizationOrder(-1)]
public class RivalsDuelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose two target creatures that share no creature types"];
}
