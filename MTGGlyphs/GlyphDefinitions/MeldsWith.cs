namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Meld's reminder of the card this one melds with: "(melds with hanweir battlements". The partner's name is open text.</summary>
/// <exampledoc>Hanweir Garrison // Hanweir, the Writhing Township</exampledoc>
/// <examplecapture>(melds with hanweir battlements</examplecapture>
public class MeldsWith : Glyph
{
    public override Nib[] Nibs => ["(melds with", Pattern("[a-z][a-z' ,-]*")];
}
