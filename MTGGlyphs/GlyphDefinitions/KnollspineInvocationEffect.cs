namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Knollspine Invocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Knollspine Invocation</exampledoc>
[Dependent]
public class KnollspineInvocationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals x damage to any target"];
}
