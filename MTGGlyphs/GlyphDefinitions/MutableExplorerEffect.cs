namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mutable Explorer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mutable Explorer</exampledoc>
[Dependent]
public class MutableExplorerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's a land with \"{t}: add {c}\" and \"{1}: this token becomes a 2/2 creature with all creature types until end of turn"];
}
