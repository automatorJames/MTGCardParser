namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Grinning Totem, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grinning Totem</exampledoc>
[Dependent]
public class GrinningTotemEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you haven't played it, put it into its owner's graveyard"];
}
