namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gate Colossus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gate Colossus</exampledoc>
[Dependent]
public class GateColossusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put this card from your graveyard on top of your library"];
}
