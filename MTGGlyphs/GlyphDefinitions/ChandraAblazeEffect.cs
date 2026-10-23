namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra Ablaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra Ablaze</exampledoc>
[Dependent]
public class ChandraAblazeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cast any number of red instant and/or sorcery cards from your graveyard without paying their mana costs"];
}
