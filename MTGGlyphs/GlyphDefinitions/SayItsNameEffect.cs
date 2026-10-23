namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Say Its Name, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Say Its Name</exampledoc>
/// <examplecapture>search your graveyard, hand, and/or library for a card named altanak, the thrice-called and put it onto the battlefield</examplecapture>
[Dependent]
public class SayItsNameEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your graveyard, hand, and/or library for a card named altanak, the thrice-called and put it onto the battlefield"];
}
