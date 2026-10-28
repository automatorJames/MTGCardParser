namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Martyrdom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Martyrdom</exampledoc>
[TokenizationOrder(-1)]
public class MartyrdomRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only you may activate this ability"];
}
