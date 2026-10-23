namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Captured by the Consulate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Captured by the Consulate</exampledoc>
[Dependent]
public class CapturedByTheConsulateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has a single target, change the target to enchanted creature if able"];
}
