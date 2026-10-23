namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nine-Lives Familiar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nine-Lives Familiar</exampledoc>
/// <examplecapture>if it had a revival counter on it, return it to the battlefield with one fewer revival counter on it at the beginning of the next end step</examplecapture>
[Dependent]
public class NineLivesFamiliarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it had a revival counter on it, return it to the battlefield with one fewer revival counter on it at the beginning of the next end step"];
}
