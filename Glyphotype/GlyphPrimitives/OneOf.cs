namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// The generic one-of primitives: exactly one of two to twelve alternative types matches, and the alternatives are
/// told apart by type alone (so none may repeat - see <see cref="Glyph.GetOneOfTypeArgumentError"/>). Alternatives
/// that share a type, or want names of their own, are a <see cref="GlyphOneOf"/> instead. The arities are listed
/// in <see cref="OneOfBase.GenericDefinitions"/>.
/// </summary>
public class OneOf<T1, T2> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
}

public class OneOf<T1, T2, T3> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
}

public class OneOf<T1, T2, T3, T4> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
    public T8 Alternative8 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
    public T8 Alternative8 { get; set; }
    public T9 Alternative9 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
    public T8 Alternative8 { get; set; }
    public T9 Alternative9 { get; set; }
    public T10 Alternative10 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
    public T8 Alternative8 { get; set; }
    public T9 Alternative9 { get; set; }
    public T10 Alternative10 { get; set; }
    public T11 Alternative11 { get; set; }
}

public class OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : OneOfBase
{
    public T1 Alternative1 { get; set; }
    public T2 Alternative2 { get; set; }
    public T3 Alternative3 { get; set; }
    public T4 Alternative4 { get; set; }
    public T5 Alternative5 { get; set; }
    public T6 Alternative6 { get; set; }
    public T7 Alternative7 { get; set; }
    public T8 Alternative8 { get; set; }
    public T9 Alternative9 { get; set; }
    public T10 Alternative10 { get; set; }
    public T11 Alternative11 { get; set; }
    public T12 Alternative12 { get; set; }
}
