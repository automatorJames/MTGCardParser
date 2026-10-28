namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ashiok, Nightmare Weaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Nightmare Weaver</exampledoc>
[Dependent]
public class AshiokNightmareWeaverEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a creature card with mana value x exiled with ashiok onto the battlefield under your control"];
}
