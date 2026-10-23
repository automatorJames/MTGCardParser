namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Unbeatable Squirrel Girl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Unbeatable Squirrel Girl</exampledoc>
[TokenizationOrder(-1)]
public class TheUnbeatableSquirrelGirlRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["do you like squirrels? — whenever", Nib.This, "enters or attacks, create a 1/1 green squirrel creature token"];
}
