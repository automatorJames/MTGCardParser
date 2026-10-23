namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Unyaro Griffin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unyaro Griffin</exampledoc>
[Dependent]
public class UnyaroGriffinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target red instant or sorcery spell"];
}
