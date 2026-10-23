namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An animated permanent keeping its abilities, as reminder text says: "(it) retains its abilities".</summary>
/// <exampledoc>Karn's Touch</exampledoc>
/// <examplecapture>retains its abilities</examplecapture>
[Dependent]
public class RetainsAbilities : Glyph, IPredicate
{
    public override Nib[] Nibs => ["retains its abilities"];
}
