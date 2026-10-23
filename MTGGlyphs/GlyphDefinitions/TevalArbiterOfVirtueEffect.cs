namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Teval, Arbiter of Virtue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teval, Arbiter of Virtue</exampledoc>
[Dependent]
public class TevalArbiterOfVirtueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each card you exile from your graveyard while casting those spells pays for {1}"];
}
