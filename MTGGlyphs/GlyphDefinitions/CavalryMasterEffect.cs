namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cavalry Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavalry Master</exampledoc>
[Dependent]
public class CavalryMasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instance of flanking triggers separately"];
}
