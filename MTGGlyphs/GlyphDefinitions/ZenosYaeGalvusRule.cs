namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zenos yae Galvus // Shinryu, Transcendent Rival, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zenos yae Galvus // Shinryu, Transcendent Rival</exampledoc>
[TokenizationOrder(-1)]
public class ZenosYaeGalvusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["my first friend — when zenos yae galvus enters, choose a creature an opponent controls"];
}
