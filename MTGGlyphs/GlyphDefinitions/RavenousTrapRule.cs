namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ravenous Trap, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ravenous Trap</exampledoc>
/// <examplecapture>if an opponent had three or more cards put into their graveyard from anywhere this turn, you may pay {0} rather than pay this spell's mana cost</examplecapture>
[TokenizationOrder(-1)]
public class RavenousTrapRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent had three or more cards put into their graveyard from anywhere this turn, you may pay {0} rather than pay this spell's mana cost"];
}
