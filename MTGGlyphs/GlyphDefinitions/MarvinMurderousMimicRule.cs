namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Marvin, Murderous Mimic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Marvin, Murderous Mimic</exampledoc>
[TokenizationOrder(-1)]
public class MarvinMurderousMimicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["marvin has all activated abilities of creatures you control that don't have the same name as this creature"];
}
