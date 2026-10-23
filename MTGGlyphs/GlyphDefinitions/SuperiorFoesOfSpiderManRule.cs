namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Superior Foes of Spider-Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Superior Foes of Spider-Man</exampledoc>
[TokenizationOrder(-1)]
public class SuperiorFoesOfSpiderManRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you may play that card until you exile another card with this creature"];
}
