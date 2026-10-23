namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hollow Warrior, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hollow Warrior</exampledoc>
[TokenizationOrder(-1)]
public class HollowWarriorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't attack or block unless you tap an untapped creature you control not declared as an attacking or blocking creature this combat"];
}
