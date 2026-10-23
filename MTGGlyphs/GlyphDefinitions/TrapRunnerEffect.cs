namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Trap Runner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trap Runner</exampledoc>
[Dependent]
public class TrapRunnerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability works on creatures that can't be blocked"];
}
