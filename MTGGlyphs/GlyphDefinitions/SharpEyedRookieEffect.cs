namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sharp-Eyed Rookie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sharp-Eyed Rookie</exampledoc>
/// <examplecapture>if its power is greater than this creature's power or its toughness is greater than this creature's toughness, put a +1/+1 counter on this creature and investigate</examplecapture>
[Dependent]
public class SharpEyedRookieEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if its power is greater than this creature's power or its toughness is greater than this creature's toughness, put a +1/+1 counter on this creature and investigate"];
}
