namespace Glyphotype.BackReferences;

/// <summary>
/// The document's reference to itself: <see cref="IDocument.ThisToken"/>, which <see cref="IDocument.GetFormattedLines"/>
/// puts wherever the document's name appears in its text.
/// <para>
/// Not a <see cref="BackReference"/>: it refers to nothing earlier, but to the document, which is known before the
/// line is read. It's a referent instead - "{this} wakes up. it sleeps all day" - and every occurrence of the token in a
/// glyph's match is one, whether it's captured by this glyph or written as a literal (see <see cref="BackReferenceResolver"/>).
/// A glyph uses this one where the document is one of several things that can fill a property, e.g. a
/// <see cref="OneOf{T1,T2}"/> of it and another subject; where it's the only one, a literal is enough.
/// </para>
/// </summary>
[Dependent]
public class This : Glyph
{
    public override Nib[] Nibs => [IDocument.ThisToken];
}
