namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kotose, the Silent Spider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kotose, the Silent Spider</exampledoc>
[Dependent]
public class KotoseTheSilentSpiderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target card other than a basic land card from an opponent's graveyard"];
}
