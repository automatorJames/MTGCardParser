namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shirei, Shizo's Caretaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shirei, Shizo's Caretaker</exampledoc>
/// <examplecapture>you may return that card to the battlefield at the beginning of the next end step if shirei is still on the battlefield</examplecapture>
[Dependent]
public class ShireiShizosCaretakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return that card to the battlefield at the beginning of the next end step if shirei is still on the battlefield"];
}
