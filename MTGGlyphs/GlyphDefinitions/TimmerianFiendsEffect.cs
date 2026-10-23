namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Timmerian Fiends, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Timmerian Fiends</exampledoc>
[Dependent]
public class TimmerianFiendsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owner of target artifact may ante the top card of their library"];
}
