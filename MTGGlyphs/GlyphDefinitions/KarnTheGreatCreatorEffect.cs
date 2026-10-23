namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Karn, the Great Creator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karn, the Great Creator</exampledoc>
[Dependent]
public class KarnTheGreatCreatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal an artifact card you own from outside the game or choose a face-up artifact card you own in exile"];
}
