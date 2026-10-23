namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jace, the Perfected Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, the Perfected Mind</exampledoc>
[Dependent]
public class JaceThePerfectedMindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player mills three times x cards"];
}
