namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ran and Shaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ran and Shaw</exampledoc>
/// <examplecapture>if you cast them and there are three or more dragon and/or lesson cards in your graveyard, create a token that's a copy of {this}, except it's not legendary</examplecapture>
[Dependent]
public class RanAndShawEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast them and there are three or more dragon and/or lesson cards in your graveyard, create a token that's a copy of", Nib.This, ", except it's not legendary"];
}
