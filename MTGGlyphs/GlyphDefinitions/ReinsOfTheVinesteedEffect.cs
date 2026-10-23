namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reins of the Vinesteed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reins of the Vinesteed</exampledoc>
/// <examplecapture>you may return this card from your graveyard to the battlefield attached to a creature that shares a creature type with that creature</examplecapture>
[Dependent]
public class ReinsOfTheVinesteedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return this card from your graveyard to the battlefield attached to a creature that shares a creature type with that creature"];
}
