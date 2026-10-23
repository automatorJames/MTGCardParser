namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rise of the Hobgoblins, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rise of the Hobgoblins</exampledoc>
[Dependent]
public class RiseOfTheHobgoblinsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["red creatures and white creatures you control gain first strike until end of turn"];
}
