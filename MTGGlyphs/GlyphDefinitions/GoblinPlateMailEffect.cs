namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goblin Plate Mail, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Plate Mail</exampledoc>
[Dependent]
public class GoblinPlateMailEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["amass goblins 1, then attach this equipment to the amassed army"];
}
