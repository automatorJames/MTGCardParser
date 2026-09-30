namespace Glyphotype.Definitions;

public enum TypeReferenceKind
{
    /// <summary>A <see cref="GlyphDefinition"/>, by name.</summary>
    Glyph,

    /// <summary>A <see cref="VocabularyDefinition"/> (an enum), by name.</summary>
    Vocabulary,

    /// <summary>A presence flag: true exactly when its pattern matched.</summary>
    Bool,

    /// <summary>A parsed open-vocabulary value, by its <see cref="PrimitiveTerminal.DisplayName"/> (e.g. "int").</summary>
    Primitive,

    /// <summary>A <see cref="DynamicGlyph"/>: resolved at match time by tokenizing its captured text.</summary>
    Dynamic,

    /// <summary>A generic <see cref="OneOf{T1,T2}"/>/<see cref="OneOf{T1,T2,T3}"/> over two or three alternatives.</summary>
    OneOf,

    /// <summary>A <see cref="CompoundOf{T}"/> over one item type.</summary>
    CompoundOf,

    /// <summary>A <see cref="ManyOf{T}"/> over one item type.</summary>
    ManyOf,

    /// <summary>An <see cref="OptionalOf{T}"/> over one glyph type.</summary>
    OptionalOf,
}

/// <summary>
/// The type of a <see cref="PropertyDefinition"/> (or the primitive a <see cref="GlyphDefinition"/> aliases),
/// referring to other definitions by name rather than by CLR <see cref="Type"/> - so a definition can refer to
/// glyphs and vocabularies that don't exist as types yet.
/// <para>
/// Compared structurally, unlike the other definition records: two references are equal when they'd resolve to
/// the same type.
/// </para>
/// </summary>
public sealed record TypeReference
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public TypeReferenceKind Kind { get; init; }

    /// <summary>The referenced definition's name for <see cref="TypeReferenceKind.Glyph"/>/<see cref="TypeReferenceKind.Vocabulary"/>, or the primitive's display name for <see cref="TypeReferenceKind.Primitive"/>. Null otherwise.</summary>
    public string Name { get; init; }

    /// <summary>The type arguments of a generic primitive (<see cref="TypeReferenceKind.OneOf"/> and the rest). Empty otherwise.</summary>
    public IReadOnlyList<TypeReference> Arguments { get; init; } = [];

    /// <summary>Whether a value-typed reference (vocabulary, bool, primitive) is nullable - see <see cref="Glyph.GetNullabilityError"/>.</summary>
    public bool IsNullable { get; init; }

    public static TypeReference Glyph(string name) => new() { Kind = TypeReferenceKind.Glyph, Name = name };
    public static TypeReference Vocabulary(string name) => new() { Kind = TypeReferenceKind.Vocabulary, Name = name };
    public static TypeReference Primitive(string displayName) => new() { Kind = TypeReferenceKind.Primitive, Name = displayName };
    public static TypeReference Bool { get; } = new() { Kind = TypeReferenceKind.Bool };
    public static TypeReference Dynamic { get; } = new() { Kind = TypeReferenceKind.Dynamic };
    public static TypeReference OneOf(params TypeReference[] alternatives) => new() { Kind = TypeReferenceKind.OneOf, Arguments = alternatives };
    public static TypeReference CompoundOf(TypeReference item) => new() { Kind = TypeReferenceKind.CompoundOf, Arguments = [item] };
    public static TypeReference ManyOf(TypeReference item) => new() { Kind = TypeReferenceKind.ManyOf, Arguments = [item] };
    public static TypeReference OptionalOf(TypeReference item) => new() { Kind = TypeReferenceKind.OptionalOf, Arguments = [item] };

    /// <summary>The reference to an existing CLR <paramref name="type"/>, which must be something a glyph property can be.</summary>
    public static TypeReference FromType(Type type) => DefinitionReader.ReadTypeReference(type);

    public TypeReference AsNullable() => this with { IsNullable = true };

    /// <summary>Whether this is a value type in C# - the only kinds <see cref="IsNullable"/> means anything for.</summary>
    [JsonIgnore]
    public bool IsValueType => Kind is TypeReferenceKind.Vocabulary or TypeReferenceKind.Bool or TypeReferenceKind.Primitive;

    /// <summary>This reference and every reference nested in its <see cref="Arguments"/>, depth first.</summary>
    public IEnumerable<TypeReference> SelfAndDescendants() =>
        Arguments.SelectMany(x => x.SelfAndDescendants()).Prepend(this);

    /// <summary>The reference as C# would spell it, e.g. <c>OneOf&lt;Animal?, Person?&gt;</c>.</summary>
    public override string ToString()
    {
        var name = Kind switch
        {
            TypeReferenceKind.Glyph or TypeReferenceKind.Vocabulary or TypeReferenceKind.Primitive => Name,
            TypeReferenceKind.Bool => "bool",
            TypeReferenceKind.Dynamic => nameof(DynamicGlyph),
            _ => $"{Kind}<{string.Join(", ", Arguments)}>",
        };

        return IsNullable && IsValueType ? name + "?" : name;
    }

    public bool Equals(TypeReference other) =>
        other is not null
        && Kind == other.Kind
        && Name == other.Name
        && IsNullable == other.IsNullable
        && Arguments.SequenceEqual(other.Arguments);

    public override int GetHashCode() => ToString().GetHashCode();
}
