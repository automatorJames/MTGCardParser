using System.Reflection.Emit;

namespace Glyphotype.Definitions;

/// <summary>
/// Compiles a <see cref="GrammarDefinition"/> into runtime CLR types - marker interfaces, enums and Glyph classes,
/// each built exactly as its C# source would declare it - ready for a <see cref="GlyphGrammar"/> (see
/// <see cref="GlyphGrammar.FromDefinition"/>). Every call emits into an assembly of its own, so the same names
/// can be emitted any number of times.
/// <para>
/// Emitting doesn't validate the grammar - <see cref="GlyphGrammar"/>'s construction does that, as it would for
/// hand-written types. It only refuses what can't be built at all: a name that resolves to nothing, a reference
/// cycle, a nib naming a property that isn't declared.
/// </para>
/// </summary>
public static class GrammarEmitter
{
    static readonly ConstructorInfo _nibCtor = typeof(Nib).GetConstructor([typeof(string)]);
    static readonly ConstructorInfo _patternNibCtor = typeof(PatternNib).GetConstructor([typeof(string)]);
    static readonly ConstructorInfo _alternativesNibCtor = typeof(NibAlternatives).GetConstructor([typeof(string[])]);
    static readonly ConstructorInfo _optionalNibCtor = typeof(OptionalNib).GetConstructor([typeof(Nib)]);
    static readonly ConstructorInfo _pluralNibCtor = typeof(PluralNib).GetConstructor([typeof(Nib)]);
    static readonly MethodInfo _propMethod = typeof(Glyph).GetMethod(nameof(Glyph.Prop));
    static readonly MethodInfo _thisNibGetter = typeof(Nib).GetProperty(nameof(Nib.This)).GetMethod;

    /// <summary>
    /// Emits every marker, vocabulary and glyph in <paramref name="grammar"/>, returning the emitted types.
    /// </summary>
    /// <param name="knownTypes">
    /// Existing types that references to names <paramref name="grammar"/> doesn't define resolve against - e.g. a
    /// grammar's <see cref="GlyphGrammar.Types"/>, when defining one new glyph on top of it. The enums, markers and
    /// glyphs those types refer to are known too. Glyphotype's own enums (e.g. <see cref="Conjunction"/>) and glyphs (e.g. <see cref="It"/>) always are.
    /// A name defined in <paramref name="grammar"/> takes precedence over a known type of the same name.
    /// </param>
    public static IReadOnlyList<Type> Emit(GrammarDefinition grammar, IEnumerable<Type> knownTypes = null)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Glyphotype.Emitted.{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
        _emittedAssemblies[assembly.GetName().Name] = assembly;

        return new Emission(assembly.DefineDynamicModule("Main"), IndexKnownTypes(knownTypes ?? [])).EmitAll(grammar);
    }

    /// <summary>
    /// Every assembly <see cref="Emit"/> has built, by name. An attribute argument naming a type (e.g.
    /// <see cref="TypeFilterAttribute"/>'s marker) is stored as an assembly-qualified name, which the runtime
    /// resolves by loading that assembly by name when the attribute is read - and it can't find a dynamic
    /// assembly that way on its own.
    /// </summary>
    static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Assembly> _emittedAssemblies = new();

    static GrammarEmitter()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            _emittedAssemblies.GetValueOrDefault(new AssemblyName(args.Name).Name);
    }

    sealed class Emission(ModuleBuilder module, Dictionary<string, Type> knownTypes)
    {
        readonly Dictionary<string, Type> _emitted = [];

        public IReadOnlyList<Type> EmitAll(GrammarDefinition grammar)
        {
            foreach (var marker in grammar.Markers)
                Add(module.DefineType(marker, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract).CreateType());

            foreach (var vocabulary in grammar.Vocabularies)
                Add(EmitVocabulary(vocabulary));

            foreach (var glyph in OrderByDependency(grammar.Glyphs))
                Add(EmitGlyph(glyph));

            return _emitted.Values.ToList();
        }

        void Add(Type type)
        {
            if (!_emitted.TryAdd(type.Name, type))
                throw new InvalidOperationException($"The grammar defines '{type.Name}' more than once");
        }

        Type EmitVocabulary(VocabularyDefinition vocabulary)
        {
            var enumBuilder = module.DefineEnum(vocabulary.Name, TypeAttributes.Public, typeof(int));

            if (vocabulary.IsOptionalPlural)
                enumBuilder.SetCustomAttribute(Attribute<OptionalPluralAttribute>());

            long value = 0;

            foreach (var member in vocabulary.Members)
            {
                value = member.Value ?? value;
                var field = enumBuilder.DefineLiteral(member.Name, checked((int)value));

                if (member.Patterns.Count > 0)
                    field.SetCustomAttribute(Attribute<RegexPatternAttribute>((object)member.Patterns.ToArray()));

                if (member.Color is not null)
                    field.SetCustomAttribute(Attribute<ColorAttribute>(member.Color));

                value++;
            }

            return enumBuilder.CreateType();
        }

        Type EmitGlyph(GlyphDefinition glyph)
        {
            var baseType = glyph.Kind switch
            {
                GlyphKind.Glyph => typeof(Glyph),
                GlyphKind.GlyphOneOf => typeof(GlyphOneOf),
                GlyphKind.BackReference when glyph.ReferenceKind is { } referenceKind => typeof(BackReference<>).MakeGenericType(Resolve(referenceKind)),
                GlyphKind.BackReference => typeof(BackReference),
                GlyphKind.Alias when glyph.AliasOf is { Kind: TypeReferenceKind.OneOf or TypeReferenceKind.CompoundOf or TypeReferenceKind.ManyOf or TypeReferenceKind.OptionalOf } => Resolve(glyph.AliasOf),
                _ => throw new InvalidOperationException($"{glyph.Name} is an alias, but not of a generic primitive (OneOf, CompoundOf, ManyOf, OptionalOf)"),
            };

            var typeBuilder = module.DefineType(glyph.Name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit, baseType);

            foreach (var marker in glyph.Markers)
                typeBuilder.AddInterfaceImplementation(ResolveName(marker));

            foreach (var attribute in GetClassAttributes(glyph))
                typeBuilder.SetCustomAttribute(attribute);

            foreach (var property in glyph.Properties)
                DefineAutoProperty(typeBuilder, property);

            if (glyph.Nibs.Count > 0)
                DefineGetterOverride(typeBuilder, nameof(Glyph.Nibs), typeof(Nib[]), il => EmitNibArray(il, glyph));

            if (glyph.Joiner is Joiner joiner)
                DefineGetterOverride(typeBuilder, nameof(Glyph.Joiner), typeof(Joiner), il => il.Emit(OpCodes.Ldc_I4, (int)joiner));

            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            return typeBuilder.CreateType();
        }

        IEnumerable<CustomAttributeBuilder> GetClassAttributes(GlyphDefinition glyph)
        {
            if (glyph.IsDependent)
                yield return Attribute<DependentAttribute>();

            if (glyph.SpanRule == SpanRule.PartialClause)
                yield return Attribute<AllowPartialClauseMatchAttribute>();

            if (glyph.TokenizationOrder is int order)
                yield return Attribute<TokenizationOrderAttribute>(order);

            if (glyph.Patterns.Count > 0)
                yield return Attribute<RegexPatternAttribute>((object)glyph.Patterns.ToArray());

            if (glyph.JoinedBy is Joiner joinedBy)
                yield return Attribute<JoinedByAttribute>(joinedBy);

            if (glyph.IsReferent)
                yield return Attribute<ReferentAttribute>();

            if (NumberAttribute(glyph.Number) is { } number)
                yield return number;
        }

        void DefineAutoProperty(TypeBuilder typeBuilder, PropertyDefinition property)
        {
            var type = Resolve(property.Type);
            var field = typeBuilder.DefineField($"<{property.Name}>k__BackingField", type, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, type, null);
            const MethodAttributes accessorAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            var getter = typeBuilder.DefineMethod($"get_{property.Name}", accessorAttributes, type, Type.EmptyTypes);
            var getterIl = getter.GetILGenerator();
            getterIl.Emit(OpCodes.Ldarg_0);
            getterIl.Emit(OpCodes.Ldfld, field);
            getterIl.Emit(OpCodes.Ret);

            var setter = typeBuilder.DefineMethod($"set_{property.Name}", accessorAttributes, null, [type]);
            var setterIl = setter.GetILGenerator();
            setterIl.Emit(OpCodes.Ldarg_0);
            setterIl.Emit(OpCodes.Ldarg_1);
            setterIl.Emit(OpCodes.Stfld, field);
            setterIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getter);
            propertyBuilder.SetSetMethod(setter);

            if (property.IsOptional)
                propertyBuilder.SetCustomAttribute(Attribute<OptionalAttribute>());

            if (property.AllowsUnmatched)
                propertyBuilder.SetCustomAttribute(Attribute<AllowUnmatchedAttribute>());

            if (property.Patterns.Count > 0)
                propertyBuilder.SetCustomAttribute(Attribute<RegexPatternAttribute>((object)property.Patterns.ToArray()));

            if (property.JoinedBy is Joiner joinedBy)
                propertyBuilder.SetCustomAttribute(Attribute<JoinedByAttribute>(joinedBy));

            if (property.TypeFilter is not null)
                propertyBuilder.SetCustomAttribute(Attribute<TypeFilterAttribute>(ResolveName(property.TypeFilter)));

            if (property.IsReferent)
                propertyBuilder.SetCustomAttribute(Attribute<ReferentAttribute>());

            if (NumberAttribute(property.Number) is { } number)
                propertyBuilder.SetCustomAttribute(number);

            if (property.RefersTo is not null)
                propertyBuilder.SetCustomAttribute(Attribute<RefersToAttribute>(property.RefersTo));
        }

        static void DefineGetterOverride(TypeBuilder typeBuilder, string name, Type type, Action<ILGenerator> emitValue)
        {
            var getter = typeBuilder.DefineMethod($"get_{name}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                type, Type.EmptyTypes);

            var il = getter.GetILGenerator();
            emitValue(il);
            il.Emit(OpCodes.Ret);

            typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null).SetGetMethod(getter);
        }

        /// <summary>Emits the equivalent of <c>[nib, nib, ...]</c>, each nib built the way its <see cref="Glyph"/> helper would.</summary>
        static void EmitNibArray(ILGenerator il, GlyphDefinition glyph)
        {
            var propertyNames = glyph.Properties.Select(x => x.Name).ToHashSet();

            il.Emit(OpCodes.Ldc_I4, glyph.Nibs.Count);
            il.Emit(OpCodes.Newarr, typeof(Nib));

            for (int i = 0; i < glyph.Nibs.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                EmitNib(il, glyph.Nibs[i]);
                il.Emit(OpCodes.Stelem_Ref);
            }

            void EmitNib(ILGenerator il, NibDefinition nib)
            {
                switch (nib)
                {
                    case NibDefinition.Literal literal:
                        il.Emit(OpCodes.Ldstr, literal.Text);
                        il.Emit(OpCodes.Newobj, _nibCtor);
                        break;

                    case NibDefinition.Pattern pattern:
                        il.Emit(OpCodes.Ldstr, pattern.Regex);
                        il.Emit(OpCodes.Newobj, _patternNibCtor);
                        break;

                    case NibDefinition.Alternatives alternatives:
                        EmitStringArray(il, alternatives.Texts);
                        il.Emit(OpCodes.Newobj, _alternativesNibCtor);
                        break;

                    case NibDefinition.Optional optional:
                        EmitNib(il, optional.Inner);
                        il.Emit(OpCodes.Newobj, _optionalNibCtor);
                        break;

                    case NibDefinition.Plural plural:
                        EmitNib(il, plural.Inner);
                        il.Emit(OpCodes.Newobj, _pluralNibCtor);
                        break;

                    case NibDefinition.Property property:
                        if (!propertyNames.Contains(property.Name))
                            throw new InvalidOperationException($"{glyph.Name} has a nib for property '{property.Name}', which it doesn't declare");

                        // this.Prop(null, "Name") - what Prop(Name) compiles to, the name being its caller argument expression.
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Ldstr, property.Name);
                        il.Emit(OpCodes.Call, _propMethod);
                        break;

                    case NibDefinition.This:
                        il.Emit(OpCodes.Call, _thisNibGetter);
                        break;

                    default:
                        throw new NotSupportedException($"Nib definition {nib.GetType().Name} can't be emitted");
                }
            }
        }

        static void EmitStringArray(ILGenerator il, IReadOnlyList<string> values)
        {
            il.Emit(OpCodes.Ldc_I4, values.Count);
            il.Emit(OpCodes.Newarr, typeof(string));

            for (int i = 0; i < values.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldstr, values[i]);
                il.Emit(OpCodes.Stelem_Ref);
            }
        }

        Type Resolve(TypeReference reference)
        {
            var type = reference.Kind switch
            {
                TypeReferenceKind.Glyph or TypeReferenceKind.Vocabulary => ResolveName(reference.Name),
                TypeReferenceKind.Bool => typeof(bool),
                TypeReferenceKind.Primitive => PrimitiveTerminal.TryGetType(reference.Name, out var primitive)
                    ? primitive
                    : throw new InvalidOperationException($"'{reference.Name}' isn't a supported primitive ({string.Join(", ", PrimitiveTerminal.SupportedDisplayNames)})"),
                TypeReferenceKind.Dynamic => typeof(DynamicGlyph),
                TypeReferenceKind.OneOf => (reference.Arguments.Count switch
                {
                    2 => typeof(OneOf<,>),
                    3 => typeof(OneOf<,,>),
                    _ => throw new InvalidOperationException($"A OneOf takes two or three alternatives, not {reference.Arguments.Count}"),
                }).MakeGenericType(reference.Arguments.Select(Resolve).ToArray()),
                TypeReferenceKind.CompoundOf => typeof(CompoundOf<>).MakeGenericType(ResolveSingleArgument(reference)),
                TypeReferenceKind.ManyOf => typeof(ManyOf<>).MakeGenericType(ResolveSingleArgument(reference)),
                TypeReferenceKind.OptionalOf => typeof(OptionalOf<>).MakeGenericType(ResolveSingleArgument(reference)),
                _ => throw new NotSupportedException($"Type reference kind {reference.Kind} can't be resolved"),
            };

            return reference.IsNullable && reference.IsValueType ? typeof(Nullable<>).MakeGenericType(type) : type;
        }

        Type ResolveSingleArgument(TypeReference reference) =>
            reference.Arguments.Count == 1
                ? Resolve(reference.Arguments[0])
                : throw new InvalidOperationException($"A {reference.Kind} takes one type argument, not {reference.Arguments.Count}");

        Type ResolveName(string name) =>
            _emitted.TryGetValue(name, out var type) || knownTypes.TryGetValue(name, out type)
                ? type
                : throw new InvalidOperationException($"'{name}' is neither defined in the grammar (or not before it's needed) nor among the known types");
    }

    /// <summary>
    /// <paramref name="glyphs"/>, each after every glyph it refers to, since a glyph's type must exist before
    /// anything can use it as a property type or type argument. Throws on a reference cycle.
    /// </summary>
    static List<GlyphDefinition> OrderByDependency(IReadOnlyList<GlyphDefinition> glyphs)
    {
        var byName = glyphs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First());
        var visiting = new List<string>();
        var done = new HashSet<string>();
        var ordered = new List<GlyphDefinition>();

        foreach (var glyph in glyphs)
            Visit(glyph);

        return ordered;

        void Visit(GlyphDefinition glyph)
        {
            if (done.Contains(glyph.Name))
                return;

            if (visiting.Contains(glyph.Name))
                throw new InvalidOperationException($"Circular reference detected: {string.Join(" -> ", visiting.SkipWhile(x => x != glyph.Name))} -> {glyph.Name}");

            visiting.Add(glyph.Name);

            foreach (var name in glyph.GetReferencedGlyphNames())
                if (byName.TryGetValue(name, out var referenced))
                    Visit(referenced);

            visiting.RemoveAt(visiting.Count - 1);
            done.Add(glyph.Name);
            ordered.Add(glyph);
        }
    }

    /// <summary>
    /// Every enum, interface and glyph type among <paramref name="knownTypes"/>, and everything those glyphs refer
    /// to through their properties, generic arguments, interfaces and type filters - plus Glyphotype's own enums and
    /// glyphs (e.g. the standard pronouns) - by name. Where two share a name, the first found wins.
    /// </summary>
    static Dictionary<string, Type> IndexKnownTypes(IEnumerable<Type> knownTypes)
    {
        var index = new Dictionary<string, Type>();
        var pending = new Stack<Type>(knownTypes.Concat(typeof(Glyph).Assembly.GetExportedTypes().Where(IsBuiltIn)).Reverse());

        while (pending.TryPop(out var type))
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsGenericType)
            {
                foreach (var argument in type.GetGenericArguments())
                    pending.Push(argument);

                continue;
            }

            if (!(type.IsEnum || type.IsInterface || type.IsAssignableTo(typeof(Glyph))) || !index.TryAdd(type.Name, type))
                continue;

            if (!type.IsAssignableTo(typeof(Glyph)))
                continue;

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                pending.Push(property.PropertyType);

                if (property.GetCustomAttribute<TypeFilterAttribute>()?.Type is Type filter)
                    pending.Push(filter);
            }

            foreach (var marker in type.GetInterfaces())
                pending.Push(marker);
        }

        return index;
    }

    /// <summary>Whether <paramref name="type"/>, one of Glyphotype's own, is one a grammar can refer to by name: an enum, or a concrete glyph such as <see cref="It"/>.</summary>
    internal static bool IsBuiltIn(Type type) =>
        type.IsEnum || (type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters && type.IsAssignableTo(typeof(Glyph)));

    /// <summary>The <see cref="SingularAttribute"/> or <see cref="PluralAttribute"/> declaring <paramref name="number"/>, or null for neither.</summary>
    static CustomAttributeBuilder NumberAttribute(GrammaticalNumber number) =>
        number switch
        {
            GrammaticalNumber.Singular => Attribute<SingularAttribute>(),
            GrammaticalNumber.Plural => Attribute<PluralAttribute>(),
            _ => null,
        };

    /// <summary>An attribute through its one constructor - every attribute a definition can carry has exactly one.</summary>
    static CustomAttributeBuilder Attribute<TAttribute>(params object[] arguments) where TAttribute : Attribute =>
        new(typeof(TAttribute).GetConstructors().Single(), arguments);
}
