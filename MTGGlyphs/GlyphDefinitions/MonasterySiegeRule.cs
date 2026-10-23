namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Monastery Siege, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Monastery Siege</exampledoc>
[TokenizationOrder(-1)]
public class MonasterySiegeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• dragons — spells your opponents cast that target you or a permanent you control cost {2} more to cast"];
}
