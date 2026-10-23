namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vodalian War Machine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vodalian War Machine</exampledoc>
[Dependent]
public class VodalianWarMachineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all merfolk tapped this turn to pay for its abilities"];
}
