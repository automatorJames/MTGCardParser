namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yasharn, Implacable Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yasharn, Implacable Earth</exampledoc>
[TokenizationOrder(-1)]
public class YasharnImplacableEarthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't pay life or sacrifice nonland permanents to cast spells or activate abilities"];
}
