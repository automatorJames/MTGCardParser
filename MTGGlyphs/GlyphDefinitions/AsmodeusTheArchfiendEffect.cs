namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Asmodeus the Archfiend, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Asmodeus the Archfiend</exampledoc>
[Dependent]
public class AsmodeusTheArchfiendEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all cards exiled with asmodeus to their owner's hand and you lose that much life"];
}
