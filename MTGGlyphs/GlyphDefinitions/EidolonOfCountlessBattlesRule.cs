namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eidolon of Countless Battles, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eidolon of Countless Battles</exampledoc>
[TokenizationOrder(-1)]
public class EidolonOfCountlessBattlesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature and enchanted creature each get +1/+1 for each creature you control and +1/+1 for each aura you control"];
}
