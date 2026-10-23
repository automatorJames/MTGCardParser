namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Liliana, Dreadhorde General, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana, Dreadhorde General</exampledoc>
[Dependent]
public class LilianaDreadhordeGeneralEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent chooses a permanent they control of each permanent type and sacrifices the rest"];
}
