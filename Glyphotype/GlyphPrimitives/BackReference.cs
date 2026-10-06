namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// A Glyph that refers back to something mentioned earlier - "it", "they", "that creature": an anaphor, in
/// linguistics terms. It matches like any other Glyph, from its own <see cref="Glyph.Nibs"/>; what it refers to, its
/// <see cref="Antecedent"/>, is resolved afterwards, once the whole line has been tokenized (see
/// <see cref="BackReferenceResolver"/>).
/// <para>
/// Reference is kept out of matching on purpose. A match only ever sees its own text, but an antecedent can sit
/// in an earlier clause, or in another Glyph entirely - so a Glyph spanning two clauses and two Glyphs of one
/// clause each leave their back-references to the same resolution, and how a line happens to be split into
/// Glyphs never changes what its pronouns mean.
/// </para>
/// <para>
/// What it can refer to is anything marked <see cref="ReferentAttribute"/>, plus every <c>{this}</c>. The resolver
/// binds it to the most recent one in its line that it agrees with (see <see cref="AgreesWith"/>), or leaves it
/// unresolved - never guesses. Derive from <see cref="BackReference{T}"/> to refer only to referents of kind
/// <c>T</c> ("that creature" is a <c>BackReference&lt;CardType&gt;</c>); from this class for a pronoun, which can
/// refer to anything. Where the Glyph containing a back-reference knows better, it binds it outright with
/// <see cref="RefersToAttribute"/>.
/// </para>
/// </summary>
public abstract class BackReference : Glyph
{
    /// <summary>The number the referent must have, from this type's <see cref="SingularAttribute"/> or <see cref="PluralAttribute"/>.</summary>
    public GrammaticalNumber Number => GrammaticalNumberAttribute.Of(Type);

    /// <summary>The kind of referent this refers to - the <c>T</c> of <see cref="BackReference{T}"/> - or null for any.</summary>
    public Type Kind => KindOf(Type);

    /// <summary>What this back-reference refers to: null until it's resolved, and after if nothing before it agrees with it.</summary>
    public ReferentCapture Antecedent { get; internal set; }

    /// <summary>
    /// Whether this back-reference can refer to <paramref name="referent"/>: its kind is this one's <see cref="Kind"/>
    /// (or derives from it), and neither side's number rules the other out.
    /// </summary>
    public bool AgreesWith(ReferentCapture referent) =>
        (Number == GrammaticalNumber.Unspecified || referent.Number == GrammaticalNumber.Unspecified || Number == referent.Number)
        && (Kind is null || referent.Kind is not null && referent.Kind.IsAssignableTo(Kind));

    /// <summary>The <c>T</c> of the <see cref="BackReference{T}"/> <paramref name="type"/> derives from, or null for a plain <see cref="BackReference"/>.</summary>
    public static Type KindOf(Type type)
    {
        for (var current = type; current is not null && current != typeof(BackReference); current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(BackReference<>))
                return current.GetGenericArguments()[0];

        return null;
    }

    /// <summary>
    /// An error for each <see cref="BackReference{T}"/> among <paramref name="types"/> that nothing in them could ever
    /// resolve: no <see cref="ReferentAttribute"/> class or property of its kind, no <see cref="RefersToAttribute"/>
    /// target that captures one, and it isn't <see cref="This"/> (always a referent).
    /// </summary>
    internal static IEnumerable<string> GetUnreferableErrors(IReadOnlyList<Type> types)
    {
        var properties = types.SelectMany(x => x.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)).ToList();

        var refersToTargets = properties
            .Where(x => x.IsDefined(typeof(RefersToAttribute)))
            .Select(x => x.DeclaringType.GetProperty(x.GetCustomAttribute<RefersToAttribute>().PropertyName))
            .Where(x => x is not null);

        var referentKinds = types.Where(x => x.IsDefined(typeof(ReferentAttribute)))
            .Concat(properties.Where(x => x.IsDefined(typeof(ReferentAttribute))).Concat(refersToTargets).SelectMany(x => ReferentCapture.PossibleKinds(x.PropertyType)))
            .Append(typeof(This))
            .ToHashSet();

        foreach (var type in types)
            if (KindOf(type) is Type kind && !referentKinds.Any(x => x.IsAssignableTo(kind)))
                yield return $"{type.Name}: it refers back to a {kind.Name}, but nothing in the grammar is a [Referent] of kind {kind.Name} - mark the {kind.Name} property (or glyph) it should refer to with [Referent]";
    }
}

/// <summary>
/// A back-reference that refers only to referents of kind <typeparamref name="T"/>: "that player" is a
/// <c>BackReference&lt;PlayerIdentity&gt;</c>, so it skips a more recent <c>{this}</c> or creature to find the player.
/// <typeparamref name="T"/> is an enum or a glyph type - whatever the referent's <see cref="ReferentAttribute"/>
/// property captures. For a pronoun that can only mean one kind of thing in its place ("they" in "creatures they
/// control" can only be a player), a back-reference of that kind matching just the pronoun says so.
/// </summary>
public abstract class BackReference<T> : BackReference;
