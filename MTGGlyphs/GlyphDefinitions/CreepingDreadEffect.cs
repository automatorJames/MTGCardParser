namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Creeping Dread, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Creeping Dread</exampledoc>
[Dependent]
public class CreepingDreadEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players reveal the discarded cards simultaneously"];
}
