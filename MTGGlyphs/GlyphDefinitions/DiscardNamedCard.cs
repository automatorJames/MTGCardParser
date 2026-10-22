namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Discarding a card with the source's name, as a cost (grandeur): "discard another card named {this}".</summary>
/// <exampledoc>Baru, Fist of Krosa</exampledoc>
/// <examplecapture>discard another card named {this}</examplecapture>
[Dependent]
public class DiscardNamedCard : Glyph
{
    public override Nib[] Nibs => ["discard", Alt("another", "a"), "card", Prop(Named)];

    public NamedThis Named { get; set; }
}
