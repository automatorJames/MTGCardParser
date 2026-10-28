namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falcon, Winged Wonder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falcon, Winged Wonder</exampledoc>
[TokenizationOrder(-1)]
public class FalconWingedWonderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["avian telepathy — when falcon enters, create redwing, a legendary 1/1 blue bird scout creature token with flying and \"whenever redwing attacks, surveil 1"];
}
