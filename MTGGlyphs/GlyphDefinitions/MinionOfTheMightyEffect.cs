namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Minion of the Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minion of the Mighty</exampledoc>
/// <examplecapture>if you attacked with creatures with total power 6 or greater this combat, you may put a dragon creature card from your hand onto the battlefield tapped and attacking</examplecapture>
[Dependent]
public class MinionOfTheMightyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, you may put a dragon creature card from your hand onto the battlefield tapped and attacking"];
}
