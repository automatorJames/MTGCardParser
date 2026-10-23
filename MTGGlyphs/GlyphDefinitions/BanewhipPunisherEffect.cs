namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Banewhip Punisher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Banewhip Punisher</exampledoc>
[Dependent]
public class BanewhipPunisherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature that has a -1/-1 counter on it"];
}
