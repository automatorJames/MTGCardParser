namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wiitigo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wiitigo</exampledoc>
[Dependent]
public class WiitigoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature if it has blocked or been blocked since your last upkeep"];
}
