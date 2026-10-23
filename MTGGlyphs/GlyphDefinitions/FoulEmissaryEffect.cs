namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Foul Emissary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Foul Emissary</exampledoc>
[Dependent]
public class FoulEmissaryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 3/2 colorless eldrazi horror creature token"];
}
