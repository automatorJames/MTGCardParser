namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Feather, the Redeemed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feather, the Redeemed</exampledoc>
/// <examplecapture>exile that card instead of putting it into your graveyard as it resolves</examplecapture>
[Dependent]
public class FeatherTheRedeemedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that card instead of putting it into your graveyard as it resolves"];
}
