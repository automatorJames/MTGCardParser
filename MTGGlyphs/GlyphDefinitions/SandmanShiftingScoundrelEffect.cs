namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sandman, Shifting Scoundrel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sandman, Shifting Scoundrel</exampledoc>
[Dependent]
public class SandmanShiftingScoundrelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return this card and target land card from your graveyard to the battlefield tapped"];
}
