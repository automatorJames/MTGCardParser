namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Curse of Shaken Faith, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Shaken Faith</exampledoc>
[TokenizationOrder(-1)]
public class CurseOfShakenFaithCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever enchanted player casts a spell other than the first spell they cast each turn or copies a spell, this aura deals 2 damage to them"];
}
