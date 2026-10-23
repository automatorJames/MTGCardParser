namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Karn Liberated, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karn Liberated</exampledoc>
[Dependent]
public class KarnLiberatedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["restart the game, leaving in exile all non-aura permanent cards exiled with karn"];
}
