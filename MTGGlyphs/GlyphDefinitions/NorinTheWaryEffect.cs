namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Norin the Wary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Norin the Wary</exampledoc>
[Dependent]
public class NorinTheWaryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile norin"];
}
