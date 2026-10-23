namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Syr Konrad, the Grim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Konrad, the Grim</exampledoc>
/// <examplecapture>or a creature card is put into a graveyard from anywhere other than the battlefield, or a creature card leaves your graveyard, syr konrad deals 1 damage to each opponent</examplecapture>
[Dependent]
public class SyrKonradTheGrimEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["or a creature card is put into a graveyard from anywhere other than the battlefield, or a creature card leaves your graveyard, syr konrad deals 1 damage to each opponent"];
}
