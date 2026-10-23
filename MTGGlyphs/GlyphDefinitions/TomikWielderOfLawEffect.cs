namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tomik, Wielder of Law, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tomik, Wielder of Law</exampledoc>
/// <examplecapture>if two or more of those creatures are attacking you and/or planeswalkers you control, that opponent loses 3 life and you draw a card</examplecapture>
[Dependent]
public class TomikWielderOfLawEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more of those creatures are attacking you and/or planeswalkers you control, that opponent loses 3 life and you draw a card"];
}
