namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zenos yae Galvus // Shinryu, Transcendent Rival, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zenos yae Galvus // Shinryu, Transcendent Rival</exampledoc>
[Dependent]
public class ZenosYaeGalvus2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a creature an opponent controls"];
}
