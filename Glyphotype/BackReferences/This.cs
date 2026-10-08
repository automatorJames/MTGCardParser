namespace Glyphotype.BackReferences;

/// <summary>
/// The document's reference to itself: <see cref="IDocument.ThisToken"/>, which <see cref="IDocument.GetFormattedLines"/>
/// puts wherever the document's name appears in its text.
/// <para>
/// Not a <see cref="BackReference"/>: it refers to nothing earlier, but to the document, which is known before the
/// line is read. It's a referent instead - "{this} wakes up. it sleeps all day". A glyph matches it with
/// <see cref="Nib.This"/>, where nothing needs holding on to it, or with a property, where the document is one of several
/// things that can fill it, e.g. a <see cref="OneOf{T1,T2}"/> of it and another subject. The token is never written as
/// text (see <see cref="Glyph.GetThisTokenError"/>), so every reference to the document is one of these.
/// </para>
/// </summary>
[Dependent]
[Referent(GrammaticalNumber.Singular)]
public class This : Glyph
{
    public override Nib[] Nibs => [IDocument.ThisToken];
}
