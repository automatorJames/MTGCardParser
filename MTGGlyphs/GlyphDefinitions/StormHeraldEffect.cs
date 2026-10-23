namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Storm Herald, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storm Herald</exampledoc>
[Dependent]
public class StormHeraldEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return any number of aura cards from your graveyard to the battlefield attached to creatures you control"];
}
