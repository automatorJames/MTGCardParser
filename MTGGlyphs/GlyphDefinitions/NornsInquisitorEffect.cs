namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Norn's Inquisitor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Norn's Inquisitor</exampledoc>
[Dependent]
public class NornsInquisitorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on it"];
}
