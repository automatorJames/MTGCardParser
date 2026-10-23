namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Samite Ministration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samite Ministration</exampledoc>
[Dependent]
public class SamiteMinistrationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain that much life"];
}
