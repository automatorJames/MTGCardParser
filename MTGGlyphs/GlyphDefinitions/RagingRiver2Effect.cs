namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Raging River, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raging River</exampledoc>
/// <examplecapture>each defending player divides all creatures without flying they control into a "left" pile and a "right" pile</examplecapture>
[Dependent]
public class RagingRiver2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each defending player divides all creatures without flying they control into a \"left\" pile and a \"right\" pile"];
}
