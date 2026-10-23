namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Long Feng, Grand Secretariat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Long Feng, Grand Secretariat</exampledoc>
[TokenizationOrder(-1)]
public class LongFengGrandSecretariatCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever another creature you control or a land you control is put into a graveyard from the battlefield, put a +1/+1 counter on target creature you control"];
}
