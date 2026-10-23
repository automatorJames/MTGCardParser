namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Conduit of Storms // Conduit of Emrakul, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conduit of Storms // Conduit of Emrakul</exampledoc>
[Dependent]
public class ConduitOfStormsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add {r} at the beginning of your next main phase this turn"];
}
