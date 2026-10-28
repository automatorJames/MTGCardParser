namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gollum, Riddle Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gollum, Riddle Master</exampledoc>
[TokenizationOrder(-1)]
public class GollumRiddleMasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent casts a spell with mana value of the chosen quality, choose one that hasn't been chosen —"];
}
