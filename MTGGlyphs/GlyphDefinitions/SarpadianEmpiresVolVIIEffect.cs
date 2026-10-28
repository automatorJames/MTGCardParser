namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sarpadian Empires, Vol. VII, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarpadian Empires, Vol. VII</exampledoc>
[Dependent]
public class SarpadianEmpiresVolVIIEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 creature token of the chosen color and type"];
}
