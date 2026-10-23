namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Roads Go Ever, Ever On, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Roads Go Ever, Ever On</exampledoc>
[Dependent]
public class RoadsGoEverEverOnEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature you control gets +1/+1 until end of turn for each plains you control"];
}
