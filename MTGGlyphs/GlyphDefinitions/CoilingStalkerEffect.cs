namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Coiling Stalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coiling Stalker</exampledoc>
/// <examplecapture>put a +1/+1 counter on target creature you control that doesn't have a +1/+1 counter on it</examplecapture>
[Dependent]
public class CoilingStalkerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature you control that doesn't have a +1/+1 counter on it"];
}
