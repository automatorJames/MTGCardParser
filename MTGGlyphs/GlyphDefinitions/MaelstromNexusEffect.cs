namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Maelstrom Nexus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maelstrom Nexus</exampledoc>
[Dependent]
public class MaelstromNexusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you cast your first spell, exile cards from the top of your library until you exile a nonland card that costs less"];
}
