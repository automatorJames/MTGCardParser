using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DocumentAnalysisInterface.Dialogs;

/// <summary>A run of highlighted code: its text, and the color it's shown in (null for the default text color).</summary>
public sealed record CodeToken(string Text, string Color);

/// <summary>The kind of type a name in C# source declares, which Visual Studio colors differently.</summary>
public enum CodeTypeKind { Class, Struct, Enum, Interface }

/// <summary>
/// Syntax highlighting for the definition viewer: C# in Visual Studio's default dark theme's colors, and JSON
/// pretty-printed in the usual editor vocabulary (keys, strings, numbers, and literals each their own color).
/// </summary>
public static class SyntaxHighlighter
{
    /// <summary>Visual Studio 2022's Dark theme, as it colors C# out of the box.</summary>
    public static class VisualStudio
    {
        public const string Background = "#1E1E1E";
        public const string Text = "#DCDCDC";
        public const string Keyword = "#569CD6";
        public const string ControlKeyword = "#D8A0DF";
        public const string Class = "#4EC9B0";
        public const string Struct = "#86C691";
        public const string EnumOrInterface = "#B8D7A3";
        public const string Method = "#DCDCAA";
        public const string String = "#D69D85";
        public const string StringEscape = "#FFD68F";
        public const string Number = "#B5CEA8";
        public const string Comment = "#57A64A";
    }

    /// <summary>JSON as Visual Studio (and VS Code) color it in their dark themes.</summary>
    public static class Json
    {
        public const string Key = "#9CDCFE";
        public const string String = "#CE9178";
        public const string Number = "#B5CEA8";
        public const string Literal = "#569CD6";
        public const string Punctuation = "#D4D4D4";
    }

    static readonly HashSet<string> _keywords =
    [
        "abstract", "as", "base", "bool", "byte", "char", "checked", "class", "const", "decimal", "default", "delegate",
        "double", "enum", "event", "explicit", "extern", "false", "fixed", "float", "get", "implicit", "in", "init", "int",
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "record", "ref", "sbyte", "sealed", "set", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "this", "true", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "var", "virtual", "void", "volatile", "nameof",
    ];

    static readonly HashSet<string> _controlKeywords =
    [
        "break", "case", "catch", "continue", "do", "else", "finally", "for", "foreach", "goto", "if", "return", "switch",
        "throw", "try", "while", "yield",
    ];

    static readonly Dictionary<string, CodeTypeKind> _declaringKeywords = new()
    {
        ["class"] = CodeTypeKind.Class,
        ["record"] = CodeTypeKind.Class,
        ["struct"] = CodeTypeKind.Struct,
        ["enum"] = CodeTypeKind.Enum,
        ["interface"] = CodeTypeKind.Interface,
    };

    enum Raw { Space, Comment, String, Escape, Number, Identifier, Punctuation }

    /// <summary>
    /// Highlights C# source. Visual Studio colors a type by what it is - class, enum, interface - which plain text
    /// can't say, so <paramref name="resolveType"/> names the kind of each type it knows; one it doesn't know, in a
    /// place only a type can be, is taken for a class.
    /// </summary>
    public static IReadOnlyList<CodeToken> CSharp(string code, Func<string, CodeTypeKind?> resolveType)
    {
        var raw = Lex(code);
        var tokens = new List<CodeToken>(raw.Count);

        // Indexes of the significant (non-space, non-comment) tokens, to look either side of an identifier.
        var significant = Enumerable.Range(0, raw.Count).Where(i => raw[i].Kind is not (Raw.Space or Raw.Comment)).ToList();
        var position = significant.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i);

        string Significant(int index) => index >= 0 && index < significant.Count ? raw[significant[index]].Text : null;

        bool StartsLine(int rawIndex)
        {
            for (int i = rawIndex - 1; i >= 0; i--)
            {
                if (raw[i].Kind != Raw.Space)
                    return false;

                if (raw[i].Text.Contains('\n'))
                    return true;
            }

            return true;
        }

        // Whether the significant token at an index sits in an attribute list: [Name(...), Name] at the start of a line.
        var inAttribute = new bool[significant.Count];
        for (int s = 0, depth = 0, start = -1; s < significant.Count; s++)
        {
            var text = Significant(s);

            if (start < 0 && text == "[" && StartsLine(significant[s]))
            {
                start = s;
                depth = 0;
            }
            else if (start >= 0)
            {
                if (text is "(" or "[")
                    depth++;
                else if (text is ")")
                    depth--;
                else if (text == "]" && depth-- == 0)
                    start = -1;
            }

            inAttribute[s] = start >= 0 && start != s;
        }

        // The type list after a class's colon, up to its body or the line's end.
        var inBaseList = new bool[significant.Count];
        for (int s = 0; s < significant.Count; s++)
        {
            if (Significant(s) != ":" || !Enumerable.Range(0, s).Reverse().TakeWhile(x => Significant(x) is not (";" or "{" or "}")).Any(x => _declaringKeywords.ContainsKey(Significant(x))))
                continue;

            for (int t = s + 1; t < significant.Count && Significant(t) is not ("{" or ";" or "where"); t++)
                inBaseList[t] = true;
        }

        string TypeColor(CodeTypeKind? kind) =>
            kind switch
            {
                CodeTypeKind.Enum or CodeTypeKind.Interface => VisualStudio.EnumOrInterface,
                CodeTypeKind.Struct => VisualStudio.Struct,
                _ => VisualStudio.Class,
            };

        bool IsIdentifier(string text) =>
            text is { Length: > 0 } && (char.IsLetter(text[0]) || text[0] is '_' or '@') && !_keywords.Contains(text) && !_controlKeywords.Contains(text);

        string ClassifyIdentifier(int rawIndex)
        {
            var text = raw[rawIndex].Text;

            if (_controlKeywords.Contains(text))
                return VisualStudio.ControlKeyword;

            if (_keywords.Contains(text))
                return VisualStudio.Keyword;

            var s = position[rawIndex];
            var previous = Significant(s - 1);
            var next = Significant(s + 1);

            // Declared: class Name, enum Name, interface Name.
            if (previous is not null && _declaringKeywords.TryGetValue(previous, out var declared))
                return TypeColor(declared);

            // Attributes are classes, whatever arguments follow.
            if (inAttribute[s] && previous is "[" or ",")
                return VisualStudio.Class;

            if (previous == "(" && Significant(s - 2) == "typeof")
                return TypeColor(resolveType(text));

            if (inBaseList[s])
                return TypeColor(resolveType(text));

            // A type used: Type Name, Type? Name, Type[] Name, Type<...>.
            if (IsIdentifier(next) || (next == "?" && IsIdentifier(Significant(s + 2))) || (next == "[" && Significant(s + 2) == "]") || next == "<" || previous == "<" || (previous == "," && IsInTypeArguments(s)))
                return TypeColor(resolveType(text));

            if (next == "(")
                return VisualStudio.Method;

            // Type.Member - but not a namespace's own parts.
            if (next == "." && previous is not ("namespace" or "using" or "."))
                return resolveType(text) is CodeTypeKind kind ? TypeColor(kind) : char.IsUpper(text[0]) ? VisualStudio.Class : null;

            return null;
        }

        bool IsInTypeArguments(int s)
        {
            for (int depth = 0, t = s - 1; t >= 0; t--)
            {
                var text = Significant(t);

                if (text == ">")
                    depth++;
                else if (text == "<" && depth-- == 0)
                    return true;
                else if (text is "(" or ")" or ";" or "{" or "}" or "=")
                    return false;
            }

            return false;
        }

        for (int i = 0; i < raw.Count; i++)
        {
            var (kind, text) = raw[i];

            tokens.Add(new(text, kind switch
            {
                Raw.Comment => VisualStudio.Comment,
                Raw.String => VisualStudio.String,
                Raw.Escape => VisualStudio.StringEscape,
                Raw.Number => VisualStudio.Number,
                Raw.Identifier => ClassifyIdentifier(i),
                _ => null,
            }));
        }

        return tokens;
    }

    static List<(Raw Kind, string Text)> Lex(string code)
    {
        var tokens = new List<(Raw, string)>();
        int i = 0;

        while (i < code.Length)
        {
            var c = code[i];
            var start = i;

            if (char.IsWhiteSpace(c))
            {
                while (i < code.Length && char.IsWhiteSpace(code[i]))
                    i++;

                tokens.Add((Raw.Space, code[start..i]));
            }
            else if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                while (i < code.Length && code[i] != '\n')
                    i++;

                tokens.Add((Raw.Comment, code[start..i].TrimEnd('\r')));

                if (code[start..i].EndsWith('\r'))
                    tokens.Add((Raw.Space, "\r"));
            }
            else if (c == '@' && i + 1 < code.Length && code[i + 1] == '"')
            {
                // Verbatim: no escapes, "" for a quote.
                i += 2;

                while (i < code.Length && !(code[i] == '"' && (i + 1 >= code.Length || code[i + 1] != '"')))
                    i += code[i] == '"' ? 2 : 1;

                i = Math.Min(i + 1, code.Length);
                tokens.Add((Raw.String, code[start..i]));
            }
            else if (c is '"' or '\'')
            {
                var quote = c;
                var run = new StringBuilder().Append(c);
                i++;

                while (i < code.Length && code[i] != quote && code[i] != '\n')
                {
                    if (code[i] == '\\' && i + 1 < code.Length)
                    {
                        if (run.Length > 0)
                            tokens.Add((Raw.String, run.ToString()));

                        run.Clear();

                        var length = code[i + 1] == 'u' ? Math.Min(6, code.Length - i) : 2;
                        tokens.Add((Raw.Escape, code.Substring(i, length)));
                        i += length;
                    }
                    else
                    {
                        run.Append(code[i++]);
                    }
                }

                if (i < code.Length && code[i] == quote)
                    run.Append(code[i++]);

                tokens.Add((Raw.String, run.ToString()));
            }
            else if (char.IsDigit(c))
            {
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '.' && i + 1 < code.Length && char.IsDigit(code[i + 1]) || code[i] == '_'))
                    i++;

                tokens.Add((Raw.Number, code[start..i]));
            }
            else if (char.IsLetter(c) || c is '_' or '@')
            {
                i++;

                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_'))
                    i++;

                tokens.Add((Raw.Identifier, code[start..i]));
            }
            else
            {
                tokens.Add((Raw.Punctuation, c.ToString()));
                i++;
            }
        }

        return tokens;
    }

    const int InlineWidth = 72;

    /// <summary>
    /// JSON pretty-printed and highlighted: indented two spaces, with an array or object kept on one line where that
    /// fits in <see cref="InlineWidth"/> characters. Text that isn't JSON comes back as it is, unhighlighted.
    /// </summary>
    public static IReadOnlyList<CodeToken> JsonText(string json)
    {
        JsonDocument document;

        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            return [new(json, null)];
        }

        using (document)
        {
            var tokens = new List<CodeToken>();
            WriteJson(document.RootElement, 0, tokens);
            return tokens;
        }
    }

    /// <param name="lead">How far into its line the element starts, past <paramref name="indent"/>: the width of its key.</param>
    static void WriteJson(JsonElement element, int indent, List<CodeToken> tokens, int lead = 0)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object or JsonValueKind.Array:
                var isObject = element.ValueKind == JsonValueKind.Object;
                var (open, close) = isObject ? ("{", "}") : ("[", "]");
                var items = isObject
                    ? element.EnumerateObject().Select(x => (Key: x.Name, Value: x.Value)).ToList()
                    : element.EnumerateArray().Select(x => (Key: (string)null, Value: x)).ToList();

                if (items.Count == 0)
                {
                    tokens.Add(new(open + close, Json.Punctuation));
                    return;
                }

                // On one line, if it fits there.
                var inline = new List<CodeToken> { new(open + (isObject ? " " : ""), Json.Punctuation) };

                for (int i = 0; i < items.Count; i++)
                {
                    if (i > 0)
                        inline.Add(new(", ", Json.Punctuation));

                    if (items[i].Key is string key)
                        AddKey(key, inline);

                    // Measured from nothing: a child that won't fit on one line breaks it, and this with it.
                    WriteJson(items[i].Value, 0, inline);
                }

                inline.Add(new((isObject ? " " : "") + close, Json.Punctuation));

                if (indent + lead + inline.Sum(x => x.Text.Length) <= InlineWidth && !inline.Any(x => x.Text.Contains('\n')))
                {
                    tokens.AddRange(inline);
                    return;
                }

                var inner = new string(' ', indent + 2);
                tokens.Add(new(open, Json.Punctuation));

                for (int i = 0; i < items.Count; i++)
                {
                    tokens.Add(new((i > 0 ? "," : "") + "\n" + inner, Json.Punctuation));

                    var keyWidth = 0;

                    if (items[i].Key is string key)
                        keyWidth = AddKey(key, tokens);

                    WriteJson(items[i].Value, indent + 2, tokens, keyWidth);
                }

                tokens.Add(new("\n" + new string(' ', indent) + close, Json.Punctuation));
                return;

            case JsonValueKind.String:
                tokens.Add(new(Quote(element.GetString()), Json.String));
                return;

            case JsonValueKind.Number:
                tokens.Add(new(element.GetRawText(), Json.Number));
                return;

            default:
                tokens.Add(new(element.GetRawText(), Json.Literal));
                return;
        }
    }

    static readonly JsonSerializerOptions _readable = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    /// <summary>A JSON string, escaping only what JSON needs escaped - so a regex's + or ' reads as itself, not \u002B.</summary>
    static string Quote(string text) => JsonSerializer.Serialize(text, _readable);

    /// <returns>The key's width, with its colon.</returns>
    static int AddKey(string key, List<CodeToken> tokens)
    {
        var quoted = Quote(key);
        tokens.Add(new(quoted, Json.Key));
        tokens.Add(new(": ", Json.Punctuation));
        return quoted.Length + 2;
    }

    /// <summary>The highlighted text of <paramref name="tokens"/> up to <paramref name="maxLines"/> lines, and whether any was cut.</summary>
    public static (IReadOnlyList<CodeToken> Tokens, bool Cut) FirstLines(IReadOnlyList<CodeToken> tokens, int maxLines)
    {
        var kept = new List<CodeToken>();
        var lines = 1;

        foreach (var token in tokens)
        {
            var newlines = token.Text.Count(x => x == '\n');

            if (lines + newlines <= maxLines)
            {
                kept.Add(token);
                lines += newlines;
                continue;
            }

            // Keep this token up to the last line allowed.
            var text = token.Text;
            var cutAt = -1;

            for (int i = 0, seen = 0; i < text.Length; i++)
                if (text[i] == '\n' && lines + ++seen > maxLines)
                {
                    cutAt = i;
                    break;
                }

            if (cutAt > 0)
                kept.Add(token with { Text = text[..cutAt] });

            return (kept, true);
        }

        return (kept, false);
    }
}
