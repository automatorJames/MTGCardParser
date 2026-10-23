namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Plague of Vermin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plague of Vermin</exampledoc>
[TokenizationOrder(-1)]
public class PlagueOfVermin2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until no one pays life"];
}
