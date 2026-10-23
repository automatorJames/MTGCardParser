namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror Shield, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror Shield</exampledoc>
[TokenizationOrder(-1)]
public class MirrorShieldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +0/+2 and has hexproof and \"whenever a creature with deathtouch blocks or becomes blocked by this creature, destroy that creature"];
}
