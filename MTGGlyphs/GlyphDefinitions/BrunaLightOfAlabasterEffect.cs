namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bruna, Light of Alabaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bruna, Light of Alabaster</exampledoc>
/// <examplecapture>you may attach to it any number of auras on the battlefield and you may put onto the battlefield attached to it any number of aura cards that could enchant it from your graveyard and/or hand</examplecapture>
[Dependent]
public class BrunaLightOfAlabasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may attach to it any number of auras on the battlefield and you may put onto the battlefield attached to it any number of aura cards that could enchant it from your graveyard and/or hand"];
}
