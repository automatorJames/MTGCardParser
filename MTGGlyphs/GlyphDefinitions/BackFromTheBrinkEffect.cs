namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Back from the Brink, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Back from the Brink</exampledoc>
[Dependent]
public class BackFromTheBrinkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a token that's a copy of that card"];
}
