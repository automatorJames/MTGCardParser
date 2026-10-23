namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Deathmist Raptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deathmist Raptor</exampledoc>
/// <examplecapture>you may return this card from your graveyard to the battlefield face up or face down</examplecapture>
[Dependent]
public class DeathmistRaptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return this card from your graveyard to the battlefield face up or face down"];
}
