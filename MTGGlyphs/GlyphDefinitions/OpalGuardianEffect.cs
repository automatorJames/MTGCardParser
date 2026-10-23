namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Opal Guardian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal Guardian</exampledoc>
/// <examplecapture>if this permanent is an enchantment, this enchantment becomes a 3/4 gargoyle creature with flying and protection from red</examplecapture>
[Dependent]
public class OpalGuardianEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this permanent is an enchantment, this enchantment becomes a 3/4 gargoyle creature with flying and protection from red"];
}
