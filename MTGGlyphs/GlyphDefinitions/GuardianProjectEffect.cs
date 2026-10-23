namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Guardian Project, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guardian Project</exampledoc>
/// <examplecapture>if it doesn't have the same name as another creature you control or a creature card in your graveyard, draw a card</examplecapture>
[Dependent]
public class GuardianProjectEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it doesn't have the same name as another creature you control or a creature card in your graveyard, draw a card"];
}
