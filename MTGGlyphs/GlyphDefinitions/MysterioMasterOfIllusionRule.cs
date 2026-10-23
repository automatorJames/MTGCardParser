namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mysterio, Master of Illusion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mysterio, Master of Illusion</exampledoc>
[TokenizationOrder(-1)]
public class MysterioMasterOfIllusionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile those tokens when mysterio leaves the battlefield"];
}
