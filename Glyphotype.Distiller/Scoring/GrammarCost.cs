namespace Glyphotype.Distiller.Scoring;

/// <summary>The description length of each definition in a grammar, and of the lists holding them.</summary>
public sealed record GrammarCost(
    IReadOnlyDictionary<string, double> GlyphBits,
    IReadOnlyDictionary<string, double> VocabularyBits,
    double OverheadBits)
{
    public double TotalBits => GlyphBits.Values.Sum() + VocabularyBits.Values.Sum() + OverheadBits;

    /// <summary>
    /// The description length of <paramref name="grammar"/>: the bits to transmit its definitions, given the
    /// engine (whose conventions - default nibs, friendly-cased names, joiners - are free). Literal text is
    /// spelled at <paramref name="charBits"/> per character plus a terminator; every other choice is coded
    /// uniformly among its possibilities, and every unbounded count with <see cref="CodeLength.Count"/>.
    /// <para>
    /// Names are labels, not grammar, so they're free: a reference to a glyph or vocabulary costs the choice
    /// among them, and a nib's property reference the choice among its glyph's properties. The exception is a
    /// name the engine matches as text (a vocabulary member with no patterns, a glyph with nothing else to
    /// match), which is spelled like any literal.
    /// </para>
    /// </summary>
    public static GrammarCost Of(GrammarDefinition grammar, double charBits)
    {
        var coder = new Coder(grammar, charBits);

        return new(
            grammar.Glyphs.ToDictionary(x => x.Name, coder.Glyph),
            grammar.Vocabularies.ToDictionary(x => x.Name, coder.Vocabulary),
            CodeLength.Count(grammar.Glyphs.Count) + CodeLength.Count(grammar.Vocabularies.Count) + CodeLength.Count(grammar.Markers.Count));
    }

    sealed class Coder(GrammarDefinition grammar, double charBits)
    {
        static readonly int _glyphKinds = Enum.GetValues<GlyphKind>().Length;
        static readonly int _spanRules = Enum.GetValues<SpanRule>().Length;
        static readonly int _joiners = Enum.GetValues<Joiner>().Length;
        static readonly int _nibKinds = typeof(NibDefinition).GetNestedTypes().Count(x => x.IsSubclassOf(typeof(NibDefinition)));
        static readonly int _proptionFlags = Enum.GetValues<Proptions>().Count(x => x != Proptions.None);
        static readonly int _typeReferenceKinds = Enum.GetValues<TypeReferenceKind>().Length;
        static readonly int _primitives = PrimitiveTerminal.SupportedDisplayNames.Count();

        public double Glyph(GlyphDefinition glyph)
        {
            var bits = CodeLength.Uniform(_glyphKinds);

            if (glyph.Kind == GlyphKind.Alias)
                bits += TypeReference(glyph.AliasOf);

            bits += CodeLength.Count(glyph.Nibs.Count) + glyph.Nibs.Sum(x => Nib(x, glyph));
            bits += Optional(glyph.Joiner is not null, CodeLength.Uniform(_joiners));
            bits += CodeLength.Count(glyph.Properties.Count) + glyph.Properties.Sum(Property);
            bits += CodeLength.Count(glyph.Markers.Count) + glyph.Markers.Count * CodeLength.Uniform(grammar.Markers.Count);
            bits += 1 + CodeLength.Uniform(_spanRules); // IsDependent, SpanRule
            bits += Optional(glyph.TokenizationOrder is not null, CodeLength.SignedInteger(glyph.TokenizationOrder ?? 0));
            bits += Texts(glyph.Patterns);
            bits += Optional(glyph.JoinedBy is not null, CodeLength.Uniform(_joiners));

            // With no nibs, properties or patterns, the engine matches the glyph's friendly-cased name.
            if (glyph.Nibs.Count == 0 && glyph.Properties.Count == 0 && glyph.Patterns.Count == 0)
                bits += Text(glyph.Name.ToFriendlyCase(TitleDisplayOption.Lower));

            return bits;
        }

        public double Vocabulary(VocabularyDefinition vocabulary) =>
            CodeLength.Count(vocabulary.Members.Count)
            + 1 // IsOptionalPlural
            + vocabulary.Members.Sum(member =>
                Optional(member.Value is not null, CodeLength.SignedInteger(member.Value ?? 0))
                + (member.Patterns.Count > 0
                    ? Texts(member.Patterns)
                    : CodeLength.Count(0) + Text(member.Name.ToFriendlyCase(TitleDisplayOption.Lower))));

        double Nib(NibDefinition nib, GlyphDefinition glyph) =>
            CodeLength.Uniform(_nibKinds) + nib switch
            {
                NibDefinition.Literal literal => Text(literal.Text),
                NibDefinition.Pattern pattern => Text(pattern.Regex),
                NibDefinition.Alternatives alternatives => Texts(alternatives.Texts),
                NibDefinition.Optional optional => Nib(optional.Inner, glyph),
                NibDefinition.Plural => 0,
                NibDefinition.Property => CodeLength.Uniform(glyph.Properties.Count) + _proptionFlags,
                _ => throw new NotSupportedException($"Nib definition {nib.GetType().Name} has no cost"),
            };

        double Property(PropertyDefinition property) =>
            TypeReference(property.Type)
            + 1 // IsOptional
            + Texts(property.Patterns)
            + Optional(property.JoinedBy is not null, CodeLength.Uniform(_joiners))
            + Optional(property.TypeFilter is not null, CodeLength.Uniform(grammar.Markers.Count));

        double TypeReference(TypeReference reference) =>
            CodeLength.Uniform(_typeReferenceKinds)
            + (reference.IsValueType ? 1 : 0) // IsNullable
            + reference.Kind switch
            {
                TypeReferenceKind.Glyph => CodeLength.Uniform(grammar.Glyphs.Count),
                TypeReferenceKind.Vocabulary => CodeLength.Uniform(grammar.Vocabularies.Count),
                TypeReferenceKind.Primitive => CodeLength.Uniform(_primitives),
                TypeReferenceKind.OneOf => 1, // two or three alternatives
                _ => 0,
            }
            + reference.Arguments.Sum(TypeReference);

        double Text(string text) => (text.Length + 1) * charBits;

        double Texts(IReadOnlyList<string> texts) => CodeLength.Count(texts.Count) + texts.Sum(Text);

        static double Optional(bool present, double bitsIfPresent) => 1 + (present ? bitsIfPresent : 0);
    }
}
