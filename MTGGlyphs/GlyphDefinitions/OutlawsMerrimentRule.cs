namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Outlaws' Merriment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaws' Merriment</exampledoc>
[TokenizationOrder(-1)]
public class OutlawsMerrimentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a red and white creature token with those characteristics"];
}
