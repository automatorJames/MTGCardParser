namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mirkwood Meditator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirkwood Meditator</exampledoc>
[Dependent]
public class MirkwoodMeditatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature's base power and toughness become 4/2 until end of turn"];
}
