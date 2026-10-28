namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bonny Pall, Clearcutter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bonny Pall, Clearcutter</exampledoc>
[Dependent]
public class BonnyPallClearcutterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card, then you may put a land card from your hand or graveyard onto the battlefield"];
}
