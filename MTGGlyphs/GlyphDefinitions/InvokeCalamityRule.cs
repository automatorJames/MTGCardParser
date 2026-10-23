namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invoke Calamity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invoke Calamity</exampledoc>
/// <examplecapture>you may cast up to two instant and/or sorcery spells with total mana value 6 or less from your graveyard and/or hand without paying their mana costs</examplecapture>
[TokenizationOrder(-1)]
public class InvokeCalamityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast up to two instant and/or sorcery spells with total mana value 6 or less from your graveyard and/or hand without paying their mana costs"];
}
