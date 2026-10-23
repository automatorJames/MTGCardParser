namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imprison, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imprison</exampledoc>
[TokenizationOrder(-1)]
public class ImprisonCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player activates an ability of enchanted creature with {t} in its activation cost that isn't a mana ability, you may pay {1}"];
}
