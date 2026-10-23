namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lilah, Undefeated Slickshot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lilah, Undefeated Slickshot</exampledoc>
/// <examplecapture>exile that spell instead of putting it into your graveyard as it resolves</examplecapture>
[Dependent]
public class LilahUndefeatedSlickshotEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that spell instead of putting it into your graveyard as it resolves"];
}
