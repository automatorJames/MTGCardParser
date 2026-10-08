namespace Glyphotype.Distiller.Scoring;

/// <summary>The description length of each definition in a grammar, and of the lists holding them.</summary>
public sealed record GrammarCost(
    IReadOnlyDictionary<string, double> GlyphBits,
    IReadOnlyDictionary<string, double> VocabularyBits,
    double OverheadBits)
{
    public double TotalBits => GlyphBits.Values.Sum() + VocabularyBits.Values.Sum() + OverheadBits;

    /// <summary>
    /// Every word the grammar spells out as literal text - in literal and <c>Alt</c> nibs, and in the names and plain
    /// synonyms of the vocabulary members it's charged for. Unmatched text can refer to these rather than spell them
    /// again (see <see cref="CorpusEncoding.UseGrammarWords"/>): whoever reads the corpus's description has the grammar already.
    /// </summary>
    public IReadOnlySet<string> SpelledWords { get; init; } = new HashSet<string>();

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
    /// <para>
    /// Given <paramref name="usage"/>, a vocabulary costs only the members the corpus used, and each of those only
    /// the synonyms that spelled something: the rest would be culled from a finished grammar, so they're free to
    /// keep around. A vocabulary nothing matched costs nothing.
    /// </para>
    /// </summary>
    public static GrammarCost Of(GrammarDefinition grammar, double charBits, VocabularyUsage usage = null)
    {
        var coder = new Coder(grammar, charBits, usage);

        return new(
            grammar.Glyphs.ToDictionary(x => x.Name, coder.Glyph),
            grammar.Vocabularies.ToDictionary(x => x.Name, coder.Vocabulary),
            CodeLength.Count(grammar.Glyphs.Count) + CodeLength.Count(grammar.Vocabularies.Count) + CodeLength.Count(grammar.Markers.Count))
        {
            SpelledWords = coder.SpelledWords,
        };
    }

    sealed class Coder(GrammarDefinition grammar, double charBits, VocabularyUsage usage)
    {
        static readonly TimeSpan _patternTimeout = TimeSpan.FromMilliseconds(250);
        static readonly char[] _regexMetacharacters = ['\\', '*', '+', '?', '|', '{', '[', '(', ')', '^', '$', '.'];

        /// <summary>The words of every text <see cref="Text"/> spelled - collected as they're charged, so only what's paid for counts.</summary>
        public HashSet<string> SpelledWords { get; } = [];

        static readonly int _glyphKinds = Enum.GetValues<GlyphKind>().Length;
        static readonly int _spanRules = Enum.GetValues<SpanRule>().Length;
        static readonly int _joiners = Enum.GetValues<Joiner>().Length;
        static readonly int _nibKinds = typeof(NibDefinition).GetNestedTypes().Count(x => x.IsSubclassOf(typeof(NibDefinition)));
        static readonly int _typeReferenceKinds = Enum.GetValues<TypeReferenceKind>().Length;
        static readonly int _primitives = PrimitiveTerminal.SupportedDisplayNames.Count();

        public double Glyph(GlyphDefinition glyph)
        {
            var bits = CodeLength.Uniform(_glyphKinds);

            if (glyph.Kind == GlyphKind.Alias)
                bits += TypeReference(glyph.AliasOf);

            if (glyph.Kind == GlyphKind.BackReference)
                bits += Optional(glyph.ReferenceKind is not null, glyph.ReferenceKind is null ? 0 : TypeReference(glyph.ReferenceKind));

            bits += CodeLength.Count(glyph.Nibs.Count) + glyph.Nibs.Sum(x => Nib(x, glyph));
            bits += Optional(glyph.Joiner is not null, CodeLength.Uniform(_joiners));
            bits += CodeLength.Count(glyph.Properties.Count) + glyph.Properties.Sum(Property);
            bits += CodeLength.Count(glyph.Markers.Count) + glyph.Markers.Count * CodeLength.Uniform(grammar.Markers.Count);
            bits += 1 + CodeLength.Uniform(_spanRules); // IsDependent, SpanRule
            bits += Optional(glyph.TokenizationOrder is not null, CodeLength.SignedInteger(glyph.TokenizationOrder ?? 0));
            bits += Texts(glyph.Patterns, literal: false);
            bits += Optional(glyph.JoinedBy is not null, CodeLength.Uniform(_joiners));

            // With no nibs, properties or patterns, the engine matches the glyph's friendly-cased name.
            if (glyph.Nibs.Count == 0 && glyph.Properties.Count == 0 && glyph.Patterns.Count == 0)
                bits += Text(glyph.Name.ToFriendlyCase(TitleDisplayOption.Lower));

            return bits;
        }

        public double Vocabulary(VocabularyDefinition vocabulary)
        {
            var members = usage is null ? vocabulary.Members : vocabulary.Members.Where(x => usage.IsUsed(vocabulary.Name, x.Name)).ToList();

            if (usage is not null && members.Count == 0)
                return 0;

            return CodeLength.Count(members.Count)
                + 1 // IsOptionalPlural
                + members.Sum(member =>
                    Optional(member.Value is not null, CodeLength.SignedInteger(member.Value ?? 0))
                    + (member.Patterns.Count > 0
                        ? Texts(UsedPatterns(vocabulary, member), literal: false)
                        : CodeLength.Count(0) + Text(member.Name.ToFriendlyCase(TitleDisplayOption.Lower))));
        }

        /// <summary>The member's patterns that spelled something in the corpus - all of them, without usage to go by (or if none can be told apart).</summary>
        IReadOnlyList<string> UsedPatterns(VocabularyDefinition vocabulary, VocabularyMemberDefinition member)
        {
            if (usage is null)
                return member.Patterns;

            var spellings = usage.Spellings(vocabulary.Name, member.Name);
            var plural = vocabulary.IsOptionalPlural ? "(?:e?s)?" : "";
            var used = member.Patterns.Where(pattern => spellings.Any(spelling => Spells(pattern + plural, spelling))).ToList();

            return used.Count > 0 ? used : member.Patterns;
        }

        static bool Spells(string pattern, string text)
        {
            try
            {
                return System.Text.RegularExpressions.Regex.IsMatch(text, $"^(?:{pattern})$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant, _patternTimeout);
            }
            catch (Exception exception) when (exception is ArgumentException or System.Text.RegularExpressions.RegexMatchTimeoutException)
            {
                // Can't tell: count it as used, which only ever overcharges.
                return true;
            }
        }

        double Nib(NibDefinition nib, GlyphDefinition glyph) =>
            CodeLength.Uniform(_nibKinds) + nib switch
            {
                NibDefinition.Literal literal => Text(literal.Text),
                NibDefinition.Pattern pattern => Text(pattern.Regex, literal: false),
                NibDefinition.Alternatives alternatives => Texts(alternatives.Texts),
                NibDefinition.Optional optional => Nib(optional.Inner, glyph),
                NibDefinition.Plural plural => Nib(plural.Inner, glyph),
                NibDefinition.Property => CodeLength.Uniform(glyph.Properties.Count),
                NibDefinition.This => 0,
                _ => throw new NotSupportedException($"Nib definition {nib.GetType().Name} has no cost"),
            };

        double Property(PropertyDefinition property) =>
            TypeReference(property.Type)
            + 1 // IsOptional
            + Texts(property.Patterns, literal: false)
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

        /// <summary>
        /// Spells <paramref name="text"/>. Literal text's words join <see cref="SpelledWords"/>; a regex's do only when
        /// it has no metacharacters, so it matches just what it spells (many a synonym is plain text written as a pattern).
        /// </summary>
        double Text(string text, bool literal = true)
        {
            if (literal || text.IndexOfAny(_regexMetacharacters) < 0)
                SpelledWords.UnionWith(text.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            return (text.Length + 1) * charBits;
        }

        double Texts(IReadOnlyList<string> texts, bool literal = true) => CodeLength.Count(texts.Count) + texts.Sum(x => Text(x, literal));

        static double Optional(bool present, double bitsIfPresent) => 1 + (present ? bitsIfPresent : 0);
    }
}
