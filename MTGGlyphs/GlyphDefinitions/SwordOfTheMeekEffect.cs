namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sword of the Meek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of the Meek</exampledoc>
[Dependent]
public class SwordOfTheMeekEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return this card from your graveyard to the battlefield, then attach it to that creature"];
}
