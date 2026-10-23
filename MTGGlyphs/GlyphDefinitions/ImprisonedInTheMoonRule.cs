namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imprisoned in the Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imprisoned in the Moon</exampledoc>
[TokenizationOrder(-1)]
public class ImprisonedInTheMoonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted permanent is a colorless land with \"{t}: add {c}\" and loses all other card types and abilities"];
}
