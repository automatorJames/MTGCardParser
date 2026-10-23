namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Noggle the Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noggle the Mind</exampledoc>
[Dependent]
public class NoggleTheMindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it loses all colors and all other creature types"];
}
