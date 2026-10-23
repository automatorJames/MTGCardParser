namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Evil Twin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evil Twin</exampledoc>
/// <examplecapture>you may have this creature enter as a copy of any creature on the battlefield, except it has "{u}{b}, {t}: destroy target creature with the same name as this creature</examplecapture>
[TokenizationOrder(-1)]
public class EvilTwinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature enter as a copy of any creature on the battlefield, except it has \"{u}{b}, {t}: destroy target creature with the same name as this creature"];
}
