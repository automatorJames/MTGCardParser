namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Angel of Serenity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angel of Serenity</exampledoc>
[Dependent]
public class AngelOfSerenityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile up to three other target creatures from the battlefield and/or creature cards from graveyards"];
}
