namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Roads Go Ever, Ever On, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roads Go Ever, Ever On</exampledoc>
[TokenizationOrder(-1)]
public class RoadsGoEverEverOnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iv — whenever you attack this turn, target creature you control gets +1/+1 until end of turn for each plains you control"];
}
