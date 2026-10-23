namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Acererak the Archlich, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Acererak the Archlich</exampledoc>
[Dependent]
public class AcererakTheArchlichEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you haven't completed tomb of annihilation, return acererak to its owner's hand and venture into the dungeon"];
}
