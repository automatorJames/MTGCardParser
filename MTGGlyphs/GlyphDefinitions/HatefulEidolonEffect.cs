namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hateful Eidolon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hateful Eidolon</exampledoc>
[Dependent]
public class HatefulEidolonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each aura you controlled that was attached to it"];
}
