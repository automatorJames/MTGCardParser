namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Demonlord Belzenlok, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonlord Belzenlok</exampledoc>
[TokenizationOrder(-1)]
public class DemonlordBelzenlokRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the card's mana value is 4 or greater, repeat this process"];
}
