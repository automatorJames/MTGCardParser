namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curse of Echoes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Echoes</exampledoc>
/// <examplecapture>each other player may copy that spell and may choose new targets for the copy they control</examplecapture>
[Dependent]
public class CurseOfEchoesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each other player may copy that spell and may choose new targets for the copy they control"];
}
