namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Powerleech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Powerleech</exampledoc>
[TokenizationOrder(-1)]
public class PowerleechRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an artifact an opponent controls becomes tapped or an opponent activates an artifact's ability without {t} in its activation cost, you gain 1 life"];
}
