namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moonshadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonshadow</exampledoc>
[TokenizationOrder(-1)]
public class MoonshadowCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more permanent cards are put into your graveyard from anywhere while this creature has a -1/-1 counter on it, remove a -1/-1 counter from this creature"];
}
