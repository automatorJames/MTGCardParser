namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Koll, the Forgemaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Koll, the Forgemaster</exampledoc>
[TokenizationOrder(-1)]
public class KollTheForgemasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature tokens you control that are enchanted or equipped get +1/+1"];
}
