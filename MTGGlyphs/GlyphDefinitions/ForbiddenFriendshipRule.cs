namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Forbidden Friendship, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forbidden Friendship</exampledoc>
[TokenizationOrder(-1)]
public class ForbiddenFriendshipRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 red dinosaur creature token with haste and a 1/1 white human soldier creature token"];
}
