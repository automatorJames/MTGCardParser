namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goliath Daydreamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goliath Daydreamer</exampledoc>
/// <examplecapture>exile that card with a dream counter on it instead of putting it into your graveyard as it resolves</examplecapture>
[Dependent]
public class GoliathDaydreamerEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that card with a dream counter on it instead of putting it into your graveyard as it resolves"];
}
