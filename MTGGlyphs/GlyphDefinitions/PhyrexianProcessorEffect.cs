namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Phyrexian Processor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Processor</exampledoc>
[Dependent]
public class PhyrexianProcessorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an x/x black phyrexian minion creature token, where x is the life paid as this artifact entered"];
}
