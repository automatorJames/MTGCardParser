namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zuko, Conflicted, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zuko, Conflicted</exampledoc>
[Dependent]
public class ZukoConflictedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one that hasn't been chosen and you lose 2 life —"];
}
