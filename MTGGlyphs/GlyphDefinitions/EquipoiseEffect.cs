namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Equipoise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Equipoise</exampledoc>
/// <examplecapture>for each land target player controls in excess of the number you control, choose a land that player controls, then the chosen permanents phase out</examplecapture>
[Dependent]
public class EquipoiseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each land target player controls in excess of the number you control, choose a land that player controls, then the chosen permanents phase out"];
}
