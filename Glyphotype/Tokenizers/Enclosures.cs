namespace Glyphotype.Tokenizers;

/// <summary>
/// The enclosures in a span of text - parenthesized and quoted text, e.g. reminder text "(this creature can't
/// attack.)" or a granted ability "has \"{t}: add {g}.\"" - and the view of that text a <see cref="RegexGraph"/>
/// matches against.
/// <para>
/// An enclosure holds clauses of its own, nested inside the clause around it, so the periods ending them don't end
/// the clause around it. Only a period outside every enclosure is a <see cref="ClauseBreak"/>. In
/// <see cref="MatchText"/> every other period is replaced by <see cref="EnclosedPeriod"/>, the same length, so
/// everything that reads a period as a clause boundary (a <see cref="DynamicGlyph"/>'s <c>[^.]</c>, a clause-break
/// nib's <c>\.</c>) reads only the real ones, and a period a glyph writes inside its own enclosure (see
/// <see cref="Inside"/>) matches only an enclosed one.
/// </para>
/// <para>
/// Delimiters pair up innermost first. A quote closes the innermost open quote, else opens one; a closing
/// parenthesis closes the innermost open parenthesis, abandoning anything opened since that never closed. Whatever
/// is left unpaired - a stray ")", an opening quote that never closes - is plain text, enclosing nothing.
/// </para>
/// </summary>
public sealed class Enclosures
{
    /// <summary>What an enclosed period reads as in <see cref="MatchText"/>: a private-use character no corpus text contains.</summary>
    public const char EnclosedPeriod = '';

    static readonly (char Open, char Close)[] _delimiters = [('(', ')'), ('"', '"')];

    /// <summary>The scanned text with every period inside an enclosure replaced by <see cref="EnclosedPeriod"/>. Outside the scanned span it's the text as is.</summary>
    public string MatchText { get; }

    /// <summary>The enclosures not inside another, in order: the index of each one's opening delimiter, and of its closing one.</summary>
    public IReadOnlyList<(int Open, int Close)> Outermost { get; }

    Enclosures(string matchText, IReadOnlyList<(int Open, int Close)> outermost)
    {
        MatchText = matchText;
        Outermost = outermost;
    }

    /// <summary>The enclosures in <paramref name="text"/> from <paramref name="start"/> up to (not including) <paramref name="end"/>.</summary>
    public static Enclosures Scan(string text, int start, int end)
    {
        var pairs = Pair(text, start, end);

        if (pairs.Count == 0)
            return new(text, []);

        var chars = text.ToCharArray();

        foreach (var (open, close) in pairs)
            for (int i = open + 1; i < close; i++)
                if (chars[i] == ClauseBreak.Period)
                    chars[i] = EnclosedPeriod;

        var outermost = pairs
            .Where(x => !pairs.Any(y => y.Open < x.Open && x.Close < y.Close))
            .OrderBy(x => x.Open)
            .ToList();

        return new(new string(chars), outermost);
    }

    /// <summary>
    /// Which characters of <paramref name="text"/> sit inside an enclosure it opens and closes itself - for a glyph's
    /// literal text, written across its nibs (see <see cref="GlyphNode"/>), where a period inside one is an enclosed
    /// period rather than a clause break.
    /// </summary>
    public static bool[] Inside(string text)
    {
        var inside = new bool[text.Length];

        foreach (var (open, close) in Pair(text, 0, text.Length))
            for (int i = open + 1; i < close; i++)
                inside[i] = true;

        return inside;
    }

    /// <summary>
    /// <see cref="Inside"/> for a glyph's nibs, read as the text they spell in order: per nib, which of its characters
    /// sit inside an enclosure the nibs open and close - one flag per character of a plain literal nib, and one for
    /// any other nib (a property, a pattern, an <c>Opt</c>...), which opens and closes nothing itself.
    /// </summary>
    public static IReadOnlyList<bool[]> Inside(IReadOnlyList<Nib> nibs)
    {
        var (spelled, offsets) = Spell(nibs);
        var inside = Inside(spelled);

        return nibs
            .Select((nib, i) => inside[offsets[i]..(offsets[i] + SpelledLength(nib))])
            .ToList();
    }

    /// <summary>
    /// For each of a glyph's nibs, read as in <see cref="Inside(IReadOnlyList{Nib})"/>: whether it ends with a delimiter
    /// opening an enclosure, and whether it starts with one closing an enclosure. The text inside binds to such a
    /// delimiter as it does to a parenthesis (see <see cref="JoinerRules"/>) - which, for a quote, only its pairing
    /// says: <c>"has \""</c> opens, a later <c>"\""</c> closes.
    /// </summary>
    public static IReadOnlyList<(bool EndsOpening, bool StartsClosing)> Delimiters(IReadOnlyList<Nib> nibs)
    {
        var (spelled, offsets) = Spell(nibs);
        var pairs = Pair(spelled, 0, spelled.Length);

        return nibs
            .Select((nib, i) => IsPlainLiteral(nib) && nib.Text.Length > 0
                ? (pairs.Any(x => x.Open == offsets[i] + nib.Text.Length - 1), pairs.Any(x => x.Close == offsets[i]))
                : (false, false))
            .ToList();
    }

    /// <summary>The text <paramref name="nibs"/> spell, each nib that isn't plain literal text one placeholder character, and where each nib starts in it.</summary>
    static (string Spelled, List<int> Offsets) Spell(IReadOnlyList<Nib> nibs)
    {
        var spelled = new StringBuilder();
        var offsets = new List<int>();

        foreach (var nib in nibs)
        {
            offsets.Add(spelled.Length);
            spelled.Append(IsPlainLiteral(nib) ? nib.Text : "\u0001");
        }

        return (spelled.ToString(), offsets);
    }

    static int SpelledLength(Nib nib) => IsPlainLiteral(nib) ? nib.Text.Length : 1;

    static bool IsPlainLiteral(Nib nib) => nib.GetType() == typeof(Nib);

    /// <summary>The span from <paramref name="start"/> to <paramref name="end"/> contains no part of an outermost enclosure without the whole of it.</summary>
    public bool IsBalanced(int start, int end) =>
        Outermost.All(x => x.Close < start || x.Open >= end || (x.Open >= start && x.Close < end));

    /// <summary>The outermost enclosure opening at <paramref name="index"/>, if one does.</summary>
    public bool TryGetOpeningAt(int index, out (int Open, int Close) enclosure)
    {
        foreach (var x in Outermost)
        {
            if (x.Open == index)
            {
                enclosure = x;
                return true;
            }
        }

        enclosure = default;
        return false;
    }

    static List<(int Open, int Close)> Pair(string text, int start, int end)
    {
        List<(int Open, int Close)> pairs = [];
        List<(char Close, int Index)> open = [];

        for (int i = start; i < end; i++)
        {
            var c = text[i];
            var closes = open.FindLastIndex(x => x.Close == c);

            // A quote is its own closer, so it closes an open quote when there is one; otherwise it opens.
            if (closes >= 0)
            {
                pairs.Add((open[closes].Index, i));
                open.RemoveRange(closes, open.Count - closes);
            }
            else if (_delimiters.Any(x => x.Open == c))
                open.Add((_delimiters.First(x => x.Open == c).Close, i));
        }

        return pairs;
    }
}
