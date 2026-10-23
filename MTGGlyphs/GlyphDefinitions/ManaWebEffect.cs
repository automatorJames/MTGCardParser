namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mana Web, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mana Web</exampledoc>
/// <examplecapture>tap all lands that player controls that could produce any type of mana that land could produce</examplecapture>
[Dependent]
public class ManaWebEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all lands that player controls that could produce any type of mana that land could produce"];
}
