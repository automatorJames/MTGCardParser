namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target creature": a target of one card type.</summary>
/// <exampledoc>Coral Helm</exampledoc>
/// <examplecapture>target creature</examplecapture>
public class TargetCard : Glyph
{
    public override Nib[] Nibs => ["target", Prop(CardType)];

    public CardType CardType { get; set; }
}