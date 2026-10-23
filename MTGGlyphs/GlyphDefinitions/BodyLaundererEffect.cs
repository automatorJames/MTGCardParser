namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Body Launderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Body Launderer</exampledoc>
[Dependent]
public class BodyLaundererEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return another target non-rogue creature card with equal or lesser power from your graveyard to the battlefield"];
}
