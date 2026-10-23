namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Halvar, God of Battle // Sword of the Realms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Halvar, God of Battle // Sword of the Realms</exampledoc>
[TokenizationOrder(-1)]
public class HalvarGodOfBattleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control that are enchanted or equipped have double strike"];
}
