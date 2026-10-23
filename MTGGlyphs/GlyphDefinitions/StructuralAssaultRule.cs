namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Structural Assault, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Structural Assault</exampledoc>
/// <examplecapture>destroy all artifacts, then {this} deals damage to each creature equal to the number of artifacts that were put into graveyards from the battlefield this turn</examplecapture>
[TokenizationOrder(-1)]
public class StructuralAssaultRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all artifacts, then", Nib.This, "deals damage to each creature equal to the number of artifacts that were put into graveyards from the battlefield this turn"];
}
