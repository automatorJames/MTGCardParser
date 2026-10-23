namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mythos of Illuna, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mythos of Illuna</exampledoc>
/// <examplecapture>if {r}{g} was spent to cast this spell, instead create a token that's a copy of that permanent, except the token has "when this token enters, if it's a creature, it fights up to one target creature you don't control</examplecapture>
[TokenizationOrder(-1)]
public class MythosOfIllunaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if {r}{g} was spent to cast this spell, instead create a token that's a copy of that permanent, except the token has \"when this token enters, if it's a creature, it fights up to one target creature you don't control"];
}
