namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Twilight Diviner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Twilight Diviner</exampledoc>
/// <examplecapture>if they entered or were cast from a graveyard, create a token that's a copy of one of them</examplecapture>
[Dependent]
public class TwilightDivinerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they entered or were cast from a graveyard, create a token that's a copy of one of them"];
}
