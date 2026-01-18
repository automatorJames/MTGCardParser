namespace MTGPlexer.CommonDTOs;

public class DynamicTokenType
{
    const string _namespace = "MTGPlexer.TokenUnits";
    List<string> _propertyParts = [];
    List<string> _parameterParts = [];
    string _baseTypeName;
    bool _omitParameterBlock;

    public List<DynamicSnippet> DynamicSnippets { get; }
    public string ClassName { get; }
    public string ClassStringForSavingToFile { get; private set; }
    public string ClassStringForDisplayingHtml { get; private set; }
    public string RenderedRegex { get; }

    public DynamicTokenType(string templateString, Type tokenUnitType = null, string className = null)
    {
        tokenUnitType ??= typeof(TokenUnit);
        _baseTypeName = tokenUnitType.Name;
        ClassName = className ?? $"New{tokenUnitType.Name}";
        DynamicSnippets = TemplateStringToDynamicSnippets(templateString);
        RenderedRegex = DynamicSnippetsToRegex<TokenUnit>(DynamicSnippets);
        ClassStringForSavingToFile = GetClassStringForSavingToFile(DynamicSnippets);
        ClassStringForDisplayingHtml = GetClassStringForDisplayingHtml(DynamicSnippets);

        // If parameters only consist of type, the parameters block can be omitted
        _omitParameterBlock = DynamicSnippets.All(x => x.SnippetType == DynamicSnippetType.Type);
    }

    string GetClassStringForSavingToFile(List<DynamicSnippet> dynamicSnippets)
    {
        var wordBuffer = new List<string>();

        foreach (var snippet in dynamicSnippets)
        {
            if (snippet.SnippetType == DynamicSnippetType.Type)
            {
                _parameterParts.Add($"{nameof(SnippetShortcuts.Prop)}({snippet.Text})");
                _propertyParts.Add($"public {snippet.Text} {snippet.Text} {{ get; set; }}");
            }
            else if (snippet.SnippetType == DynamicSnippetType.Method)
                _parameterParts.Add($"{snippet.Method.Name}({snippet.Text})");
            else
                _parameterParts.Add("\"" + snippet.Text + "\"");
        }

        return 
            $$"""
            namespace {{_namespace}};

            public class {{ClassName}} : {{nameof(TokenUnit)}}
            {
            {{GetSnippetsPartFormattedForFile()}}    {{string.Join("\r\n    ", _propertyParts)}}
            }
            """;

        // local helper
        string GetSnippetsPartFormattedForFile()
        {
            if (_omitParameterBlock)
                return string.Empty;

            return
                $$"""
                protected override Snippet[] Snippets => [{{string.Join(", ", _parameterParts)}}]


            """;
        }
    }

    string GetClassStringForDisplayingHtml(List<DynamicSnippet> dynamicSnippets)
    {
        var classDeclaration = $"{Span("public class")} {Span(ClassName, SpanClass.type)} {Span(":", SpanClass.identifier)} {Span(_baseTypeName, SpanClass.type)}";

        var propSection = dynamicSnippets
            .Where(x => x.SnippetType == DynamicSnippetType.Type)
            .Select(x => $"{Span("public")} {Span(x.Text, x.IsEnum ? SpanClass.enumtype : SpanClass.type)} {Span(x.Text, SpanClass.identifier)} {{ {Span("get")}{Span(";", SpanClass.identifier)} {Span("set")}{Span(";", SpanClass.identifier)} }}");

        var classPartStyled =
            $$"""
            {{classDeclaration}}
            {
            {{GetSnippetPartFormattedForHtml()}}{{string.Join("\r\n    ", propSection)}}
            }
            """;

        // Preserve C# indentation and line structure when rendering inline HTML.
        // We intentionally replace *pairs* of spaces (not single spaces) to avoid
        // breaking HTML tags/attributes, while still preventing whitespace collapse.
        // Newlines are converted to <br> for inline rendering without <pre>.
        classPartStyled = classPartStyled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");

        return classPartStyled;

        // local helper
        string GetSnippetPartFormattedForHtml()
        {
            if (_omitParameterBlock)
                return string.Empty;

            var str = $"    {Span("protected override")} {Span("Snippet", SpanClass.type)}{Span("[]")} {Span("Snippets =>", SpanClass.identifier)} {Span("[", SpanClass.identifier)}";
            str += string.Join(", ", dynamicSnippets.Select(DynamicSnippetToHtmlParameterPart));
            str += $"{Span("]", SpanClass.identifier)};";

            return str + "\r\n" + "\r\n" + "    ";

            // local helper
            string DynamicSnippetToHtmlParameterPart(DynamicSnippet snippet)
            {
                return snippet.SnippetType switch
                {
                    DynamicSnippetType.Type => $"{Span("Prop", SpanClass.method)}{Span("(" + snippet.Text + ")", SpanClass.identifier)}",
                    DynamicSnippetType.Method => $"{Span($"{snippet.Method.Name}", SpanClass.method)}{Span("(" + snippet.Text + ")", SpanClass.identifier)}",
                    DynamicSnippetType.Text => $"{Span($"\"{snippet.Text}\"", SpanClass.stringliteral)}",
                    _ => throw new NotImplementedException()
                };
            }
        }
        
        // local helper
        string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
    }

    string DynamicSnippetsToRegex<T>(List<DynamicSnippet> snippets) where T : TokenUnit
    {
        var regexSegments = snippets.Select<DynamicSnippet, RegexSegmentBase>(x =>
        {
            return x.SnippetType switch
            {
                DynamicSnippetType.Type => new TemplatePropInfo(x.Type).GetCaptureGroupPropBase(),
                DynamicSnippetType.Method => DynamicSnippetMethodToTextSegment(x),
                DynamicSnippetType.Text => new TextSegment(x.Text),
                _ => new TextSegment(x.Text),
            };
        });

        return CompositionFactory.GetComposedString(regexSegments, typeof(T));
    }

    List<DynamicSnippet> TemplateStringToDynamicSnippets(string templateString)
    {
        // Pattern to split on both "@Token" elements and "Opt(text)" elements interspered in normal text
        var splittingPattern = @"(@\w+|(?:Alt|Opt|NoSpace|Prop)\([^)]+\)|.+?(?=@\w+|(?:Alt|Opt|NoSpace|Prop)\(|$))";

        return Regex.Split(templateString, splittingPattern)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Select(x =>
            {
                // Case A: It's a Type Token (Check if it starts with @ and try to resolve)
                if (x.StartsWith("@") && TokenTypeRegistry.NameToType.TryGetValue(x[1..], out var type))
                    return new DynamicSnippet(DynamicSnippetType.Type, Text: type.Name, Type: type, IsEnum: TokenTypeRegistry.EnumRegexStrings.ContainsKey(type));
            
                // Case B: It's a Shortcut Token
                var shortcutMatch = Regex.Match(x, @"^(?<Name>Alt|Opt|NoSpace|Prop)\((?<Args>.+)\)$");
                if (shortcutMatch.Success)
                {
                    var methodName = shortcutMatch.Groups["Name"].Value;
                    var argsString = shortcutMatch.Groups["Args"].Value;
                    var method = typeof(SnippetShortcuts).GetMethod(methodName);
            
                    if (method != null)
                        return new DynamicSnippet(DynamicSnippetType.Method, Text: argsString, Method: method);
                }
            
                // Case C: Plain text (including literal spaces from the template)
                return new DynamicSnippet(DynamicSnippetType.Text, Text: x);
            }).ToList();
    }

    TextSegment DynamicSnippetMethodToTextSegment(DynamicSnippet methodSnippet)
    {
        var parameters = methodSnippet.Method.GetParameters();
        object[] invokeArgs = parameters.Length switch
        {
            1 when parameters[0].ParameterType == typeof(string[])
                => new object[] { methodSnippet.Text.Split(',').Select(s => s.Trim()).ToArray() },
            1 => new object[] { methodSnippet.Text },
            2 => new object[] { null, methodSnippet.Text },
            _ => null
        };

        if (invokeArgs != null)
        {
            var shortcutSnippet = (Snippet)methodSnippet.Method.Invoke(null, invokeArgs);
            return new TextSegment(shortcutSnippet);
        }
        else
            return new TextSegment(methodSnippet.Text);
    }
}

enum SpanClass
{
    keyword,
    type,
    enumtype,
    identifier,
    method,
    stringliteral
}