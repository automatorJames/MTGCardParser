namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crazed Firecat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crazed Firecat</exampledoc>
[TokenizationOrder(-1)]
public class CrazedFirecatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on this creature for each flip you won"];
}
