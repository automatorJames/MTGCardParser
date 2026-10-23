namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ogre Enforcer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ogre Enforcer</exampledoc>
[TokenizationOrder(-1)]
public class OgreEnforcerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can't be destroyed by lethal damage unless lethal damage dealt by a single source is marked on it"];
}
