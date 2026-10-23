namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A double-faced card's trigger on entering or on transforming into its named face: "enters or transforms into huntmaster of the fells", "enters or transforms into sygg, wanderwine wisdom". The face name stays as text (only the whole two-faced name becomes {this}), so it's kept inside this glyph; a comma inside the name is allowed only where another comma follows it.</summary>
/// <exampledoc>Huntmaster of the Fells // Ravager of the Fells</exampledoc>
/// <examplecapture>enters or transforms into huntmaster of the fells</examplecapture>
[Dependent]
public class EntersOrTransformsInto : Glyph
{
    public override Nib[] Nibs => ["enters or transforms into", Pattern("[a-z][a-z'-]*(?: [a-z][a-z'-]*){0,3}(?:, [a-z][a-z'-]*(?: [a-z][a-z'-]*){0,3}(?=,))?")];
}
