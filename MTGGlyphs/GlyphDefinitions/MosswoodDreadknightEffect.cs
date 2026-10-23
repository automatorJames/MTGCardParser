namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mosswood Dreadknight // Dread Whispers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mosswood Dreadknight // Dread Whispers</exampledoc>
[Dependent]
public class MosswoodDreadknightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast it from your graveyard as an adventure until the end of your next turn"];
}
