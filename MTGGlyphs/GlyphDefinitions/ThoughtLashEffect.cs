namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thought Lash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Lash</exampledoc>
[Dependent]
public class ThoughtLashEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player exiles all cards from their library"];
}
