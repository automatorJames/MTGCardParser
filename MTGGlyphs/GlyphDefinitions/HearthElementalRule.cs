namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hearth Elemental // Stoke Genius, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hearth Elemental // Stoke Genius</exampledoc>
/// <examplecapture>this spell costs {x} less to cast, where x is the number of cards in your graveyard that are instant cards, sorcery cards, and/or have an adventure</examplecapture>
[TokenizationOrder(-1)]
public class HearthElementalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {x} less to cast, where x is the number of cards in your graveyard that are instant cards, sorcery cards, and/or have an adventure"];
}
