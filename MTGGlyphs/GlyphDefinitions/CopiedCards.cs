namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards copied so the copies can be cast: cards named earlier ("the exiled card", "those cards", "them") or cards this permanent exiled ("each card exiled with this artifact").</summary>
/// <exampledoc>Isochron Scepter</exampledoc>
/// <examplecapture>the exiled card</examplecapture>
[Dependent]
public class CopiedCards : GlyphOneOf
{
    public ThatCardPhrase ThatCardPhrase { get; set; }
    public Them Them { get; set; }
    public CardsExiledWithThis CardsExiledWithThis { get; set; }
}
