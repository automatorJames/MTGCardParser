namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Master of the Hunt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Master of the Hunt</exampledoc>
[TokenizationOrder(-1)]
public class MasterOfTheHuntRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it has \"bands with other creatures named wolves of the hunt"];
}
