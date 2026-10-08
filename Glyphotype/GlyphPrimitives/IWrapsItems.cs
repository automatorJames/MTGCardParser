namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// A primitive that only wraps items of <typeparamref name="T"/> - <see cref="OptionalOf{T}"/>, <see cref="ManyOf{T}"/>
/// and <see cref="CompoundOf{T}"/>: what it matched is its items, so the kind of thing it is, as a referent, is theirs
/// (see <see cref="ReferentCapture.KindOf"/> and <see cref="ReferentCapture.PossibleKinds"/>).
/// </summary>
internal interface IWrapsItems<T> : IWrapsItems;

/// <summary>The items an <see cref="IWrapsItems{T}"/> matched, read without knowing its <c>T</c>.</summary>
internal interface IWrapsItems
{
    /// <summary>The items matched, in order.</summary>
    IEnumerable<object> WrappedItems { get; }
}
