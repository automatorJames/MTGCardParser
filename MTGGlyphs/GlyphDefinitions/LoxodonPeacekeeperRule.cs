namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Loxodon Peacekeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loxodon Peacekeeper</exampledoc>
[TokenizationOrder(-1)]
public class LoxodonPeacekeeperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more players are tied for lowest life total, you choose one of them, and that player gains control of this creature"];
}
