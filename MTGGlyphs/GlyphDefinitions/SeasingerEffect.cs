namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Seasinger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seasinger</exampledoc>
[Dependent]
public class SeasingerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target creature whose controller controls an island for as long as you control this creature and this creature remains tapped"];
}
