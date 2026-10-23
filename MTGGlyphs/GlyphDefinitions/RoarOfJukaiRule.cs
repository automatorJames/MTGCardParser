namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Roar of Jukai, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roar of Jukai</exampledoc>
[TokenizationOrder(-1)]
public class RoarOfJukaiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["splice onto arcane—an opponent gains 5 life"];
}
