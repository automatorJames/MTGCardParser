namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ashiok, Nightmare Weaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Nightmare Weaver</exampledoc>
[Dependent]
public class AshiokNightmareWeaverCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from all opponents' hands and graveyards"];
}
