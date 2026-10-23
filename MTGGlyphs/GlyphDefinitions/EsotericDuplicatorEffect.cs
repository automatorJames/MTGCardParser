namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Esoteric Duplicator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Esoteric Duplicator</exampledoc>
[Dependent]
public class EsotericDuplicatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {2}"];
}
