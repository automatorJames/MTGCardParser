namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reincarnation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reincarnation</exampledoc>
[Dependent]
public class ReincarnationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return a creature card from its owner's graveyard to the battlefield under the control of that creature's owner"];
}
