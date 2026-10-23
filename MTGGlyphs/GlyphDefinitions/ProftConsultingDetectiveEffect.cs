namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Proft, Consulting Detective, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Proft, Consulting Detective</exampledoc>
[Dependent]
public class ProftConsultingDetectiveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw after you scry or surveil"];
}
