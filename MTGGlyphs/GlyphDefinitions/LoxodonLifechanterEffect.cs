namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Loxodon Lifechanter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loxodon Lifechanter</exampledoc>
[Dependent]
public class LoxodonLifechanterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have your life total become the total toughness of creatures you control"];
}
