namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Exquisite Archangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exquisite Archangel</exampledoc>
[TokenizationOrder(-1)]
public class ExquisiteArchangelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would lose the game, instead exile this creature and your life total becomes equal to your starting life total"];
}
