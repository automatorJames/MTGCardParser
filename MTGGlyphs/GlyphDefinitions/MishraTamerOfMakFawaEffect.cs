namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mishra, Tamer of Mak Fawa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra, Tamer of Mak Fawa</exampledoc>
[Dependent]
public class MishraTamerOfMakFawaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return the card from your graveyard to the battlefield"];
}
