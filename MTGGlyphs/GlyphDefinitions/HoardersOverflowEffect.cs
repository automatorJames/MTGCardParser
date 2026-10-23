namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hoarder's Overflow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hoarder's Overflow</exampledoc>
[Dependent]
public class HoardersOverflowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a stash counter on it"];
}
