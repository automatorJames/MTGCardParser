namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mechtitan Core, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mechtitan Core</exampledoc>
[Dependent]
public class MechtitanCoreEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return all cards exiled with this vehicle except this card to the battlefield tapped under their owners' control"];
}
