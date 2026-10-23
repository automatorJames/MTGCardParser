namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vampire Nocturnus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vampire Nocturnus</exampledoc>
[TokenizationOrder(-1)]
public class VampireNocturnusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as the top card of your library is black, this creature and other vampire creatures you control get +2/+1 and have flying"];
}
