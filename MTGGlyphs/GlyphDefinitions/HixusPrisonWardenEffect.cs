namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hixus, Prison Warden, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hixus, Prison Warden</exampledoc>
[Dependent]
public class HixusPrisonWardenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if hixus entered this turn, exile that creature until hixus leaves the battlefield"];
}
