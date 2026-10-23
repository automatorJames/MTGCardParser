namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A double-faced card transforming itself by its front face's name ("transform storm the vault"): the corpus replaces only the whole two-faced name with {this}, so the face name stays as text - one to four words, kept inside this glyph so open slots elsewhere can't take loose words for a name.</summary>
/// <exampledoc>Storm the Vault // Vault of Catlacan</exampledoc>
/// <examplecapture>transform storm the vault</examplecapture>
[Dependent]
public class TransformByFaceName : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["transform", Pattern(@"(?!(?:this|it|him|her|them|target|that|all|each|any|up|enchanted|equipped)\b)[a-z][a-z'-]*(?: [a-z][a-z'-]*){0,3}")];
}
