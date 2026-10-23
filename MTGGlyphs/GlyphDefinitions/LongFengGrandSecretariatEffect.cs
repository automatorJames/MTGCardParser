namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Long Feng, Grand Secretariat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Long Feng, Grand Secretariat</exampledoc>
/// <examplecapture>put a +1/+1 counter on target creature you control</examplecapture>
[Dependent]
public class LongFengGrandSecretariatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature you control"];
}
