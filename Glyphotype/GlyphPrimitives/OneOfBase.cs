namespace Glyphotype.GlyphPrimitives;

public abstract class OneOfBase : Glyph
{
    public override Joiner Joiner => Joiner.Pipe;

    /// <summary>
    /// The CLR type of whichever alternative actually resolved on this instance (e.g. <c>Animal</c>
    /// for a <see cref="OneOf{T1,T2}"/> whose first slot matched) — never the nullable wrapper, and
    /// never this instance's own (often generic-mangled, e.g. "OneOf`2") type name. This isn't a
    /// proper data-path citizen; it's a display-time fact about which choice a one-of resolved to,
    /// surfaced by callers like the property table alongside the path step itself.
    /// Found by scanning this instance's alternative slots (see <see cref="GetAlternativeProps"/>) for the
    /// single one holding a non-null value — the one alternative that matched — which is the same lookup
    /// whether the slots are named Alternative1/2/3 (<see cref="OneOf{T1,T2}"/>) or given proper names by
    /// a concrete <see cref="GlyphOneOf"/>.
    /// </summary>
    public Type GetResolvedType()
    {
        var winningProp = GetAlternativeProps(GetType()).FirstOrDefault(p => p.GetValue(this) != null);
        return winningProp == null ? null : Nullable.GetUnderlyingType(winningProp.PropertyType) ?? winningProp.PropertyType;
    }

    /// <summary>
    /// <paramref name="oneOfType"/>'s alternative slots: every property declared between it and
    /// <see cref="OneOfBase"/>, base-most level first. Not just <paramref name="oneOfType"/>'s own, since
    /// the slots needn't live on the most-derived type - a pure alias like
    /// <c>class Foo : OneOf&lt;A?, B?&gt;;</c> declares none of its own, its slots being
    /// <see cref="OneOf{T1,T2}"/>'s. Excludes overrides of base members (e.g. <see cref="Glyph.Joiner"/>).
    /// </summary>
    public static PropertyInfo[] GetAlternativeProps(Type oneOfType)
    {
        var levels = new List<PropertyInfo[]>();

        for (var current = oneOfType; current != typeof(OneOfBase); current = current.BaseType)
            levels.Add(current.GetOwnProps());

        levels.Reverse();
        return levels.SelectMany(x => x).ToArray();
    }

    public override string ValidateStructure()
    {
        var graph = GlyphTypeCache.GetRegexGraph(Type);
        var props = GetAlternativeProps(Type);

        if (props.Count() < 2)
            return $"Nibs for {Type.Name} must contain at least two property references";

        if (!graph.RootNode.ValidateCapturePropertiesAreContiguous())
            return $"Nibs for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        return base.ValidateStructure();
    }
}