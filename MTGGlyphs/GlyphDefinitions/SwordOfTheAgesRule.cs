namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sword of the Ages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of the Ages</exampledoc>
[TokenizationOrder(-1)]
public class SwordOfTheAgesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{t}, sacrifice this artifact and any number of creatures you control: this artifact deals x damage to any target, where x is the total power of the creatures sacrificed this way, then exile this artifact and those creature cards"];
}
