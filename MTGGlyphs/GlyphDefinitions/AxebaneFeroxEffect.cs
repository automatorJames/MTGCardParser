namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Axebane Ferox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Axebane Ferox</exampledoc>
/// <examplecapture>whenever this creature becomes the target of a spell or ability an opponent controls, counter it unless that player exiles cards with total mana value 4 or greater from their graveyard</examplecapture>
[Dependent]
public class AxebaneFeroxEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature becomes the target of a spell or ability an opponent controls, counter it unless that player exiles cards with total mana value 4 or greater from their graveyard"];
}
