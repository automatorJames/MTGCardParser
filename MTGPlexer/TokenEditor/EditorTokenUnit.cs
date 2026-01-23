using System.ComponentModel;

namespace MTGPlexer.TokenEditor;

public class EditorTokenUnit
{
    const string _saveFileInNamespace = "MTGPlexer.TokenUnits";

    static Regex _templateSplitPattern = new Regex(
        @"
        (?<Method>                  # Start Method Group
            @(?<MethodName>\w+)     # Match @name
            \(                      # Open parenthesis
                (?:                 # Start non-capturing group for args
                    \s* 
                    (?<Arg>[^,)]+)  # Capture the argument content
                    \s* 
                    (?: , | $ | (?=\)) ) # Followed by comma, end of string, or lookahead for )
                    \s*
                )*                  # Repeat for any number of args
            \)                      # Close parenthesis
        )                           # End Method Group
        |
        (?<Type>                    # Start Type Group
            @(?:
                (?<Wrapper>\w+)<(?<Base>\w+)>  # Generic: @Wrapper<Base>
                |
                (?<Base>\w+)                   # Plain: @Base
            )
        )                           # End Type Group
        |
        (?<Plain>                   # Start Plain Group
            (?: (?! @\w ) . )+      # Match any char that isn't the start of a @Symbol
        )",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

    public ProcessedLine LineMetadata { get; }
    public Type TokenUnitType { get; } = typeof(TokenUnit);
    public List<EditorSnippet> EditorSnippets { get; private set; }
    public string ClassName { get; private set; }
    public string ClassStringForSavingToFile { get; private set; }
    public string ClassStringForDisplayingHtml { get; private set; }
    public string RenderedRegex { get; private set; }

    public EditorTokenUnit(ProcessedLine lineMetadata)
    {
        LineMetadata = lineMetadata;
        Update(string.Empty);
    }

    public void Update(string templateString, string preferredClassName = null)
    {
        EditorSnippets = DigestTemplateStringToSnippets(templateString);
        ClassName = preferredClassName ?? GetSuggestedClassName();

        if (string.IsNullOrWhiteSpace(ClassName))
            ClassName = $"New{TokenUnitType.Name}";

        RenderedRegex = CompositionFactory.GetComposedString(EditorSnippets.Select(x => x.GetRegexSegment()), TokenUnitType);
        ClassStringForSavingToFile = GetClassStringForSavingToFile();
        ClassStringForDisplayingHtml = GetClassStringForDisplayingHtml();
    }

    string GetClassStringForSavingToFile()
    {
        return 
            $$"""
            namespace {{_saveFileInNamespace}};

            public class {{ClassName}} : {{nameof(TokenUnit)}}
            {
                protected override Snippet[] Snippets => [{{string.Join(", ", EditorSnippets.Select(x => x.ParameterRepresentation))}}];

                {{string.Join("\r\n    ", EditorSnippets.OfType<EditorPropertySnippet>().Select(x => x.GetPropertyLineRepresentation()))}}
            }
            """;
    }

    string GetClassStringForDisplayingHtml()
    {
        var classDeclaration = $"{Span("public class")} {Span(ClassName, SpanClass.type)} {Span(":", SpanClass.identifier)} {Span(TokenUnitType.Name, SpanClass.type)}";

        var snippetSection = 
            $"{Span("protected override")} {Span("Snippet", SpanClass.type)}{Span("[]")} {Span("Snippets =>", SpanClass.identifier)} {Span("[", SpanClass.identifier)}"
            + string.Join(", ", EditorSnippets.Select(x => x.GetParameterHtmlRepresentation()))
            + $"{Span("]", SpanClass.identifier)};";

        var propDeclarations = EditorSnippets
            .OfType<EditorPropertySnippet>()
            .Select(x => x.GetPropertyLineHtmlRepresentation());

        var classPartStyled =
            $$"""
            {{classDeclaration}}
            {
                {{snippetSection}}

                {{string.Join("\r\n    ", propDeclarations)}}
            }
            """;

        // Preserve C# indentation and line structure when rendering inline HTML.
        // We intentionally replace *pairs* of spaces (not single spaces) to avoid
        // breaking HTML tags/attributes, while still preventing whitespace collapse.
        // Newlines are converted to <br> for inline rendering without <pre>.
        classPartStyled = classPartStyled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");

        return classPartStyled;
    }

    List<EditorSnippet> DigestTemplateStringToSnippets(string templateString)
    {
        List <EditorSnippet> list = [];
        var matches = _templateSplitPattern.Matches(templateString);

        foreach (Match match in matches)
        {
            string snippet = match.Value.Trim();
            if (string.IsNullOrEmpty(snippet)) continue;

            if (match.Groups["Method"].Success)
            {
                var name = match.Groups["MethodName"].Value;
                var args = match.Groups["Arg"].Captures.Cast<Capture>().Select(c => c.Value.Trim()).ToArray();

                if (!Enum.TryParse<ShortcutSnippetMethod>(name, out var parsedMethodType))
                    continue;
                    //throw new Exception($"'{name}' isn't a valid {nameof(ShortcutSnippetMethod)}");

                list.Add(new EditorMethodSnippet(parsedMethodType, args));
            }
            else if (match.Groups["Type"].Success)
            {
                var wrapper = match.Groups["Wrapper"].Value; // Empty if no < >
                var baseType = match.Groups["Base"].Value;

                XOfType xOfType = XOfType.None;

                if (!string.IsNullOrEmpty(wrapper) && !Enum.TryParse<XOfType>(wrapper, out xOfType))
                    continue;
                    //throw new Exception($"'{wrapper}' isn't a valid {nameof(XOfType)}");

                if (!TokenTypeRegistry.NameToType.TryGetValue(baseType, out Type parsedBaseType))
                    continue;
                    //throw new Exception($"'{baseType}' isn't a registered type");

                list.Add(new EditorPropertySnippet(parsedBaseType, xOfType));
            }
            else if (match.Groups["Plain"].Success)
                list.Add(new EditorTextSnippet(match.Value.Trim()));
        }

        return list;
    }

    string GetSuggestedClassName()
    {
        string str = "";

        foreach (var snippet in EditorSnippets)
        {
            if (snippet is EditorPropertySnippet propertySnippet)
                str += propertySnippet.PropertyNameRepresentation;
            else if (snippet is EditorTextSnippet textSnippet)
            {
                var rawTextParts = textSnippet.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Regex.Replace(x, @"[^\w]+", ""))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => char.ToUpper(x[0]) + x[1..].ToLower());

                str += string.Join("", rawTextParts);
            }
        }

        return str;
    }

    public void RemoveSnippet(Guid snippetId)
    {
        EditorSnippets.RemoveAll(s => s.Id == snippetId);
        var newTemplateString = GetTemplateString();
        Update(newTemplateString);
    }

    public string GetTemplateString() =>
        string.Join(' ', EditorSnippets.Select(x => x.EditorRepresentation));

    static string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
}

public enum TokenModifier
{
    [Description("Match Exactly One")]
    None,
    [Description("Optional (?)")]
    Optional,
    [Description("Zero or More (*)")]
    ZeroOrMore,
    [Description("One or More (+)")]
    OneOrMore
}

public enum SpanClass
{
    keyword,
    type,
    enumtype,
    identifier,
    method,
    stringliteral
}