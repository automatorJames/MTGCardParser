namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Phyrexian Grimoire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Grimoire</exampledoc>
[Dependent]
public class PhyrexianGrimoireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent chooses one of the top two cards of your graveyard"];
}
