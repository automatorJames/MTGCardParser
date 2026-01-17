namespace MTGPlexer.CommonDTOs;

public class DynamicTokenType
{
    protected List<string> _referencedTypeNames = [];
    protected List<string> _propertyParts = [];
    protected List<string> _parameterParts = [];
    public List<(string Val, bool IsType)> CombinedParts { get; set; } = [];
    public List<string> DynamicSnippets { get; } = [];
    public string ClassName { get; }
    public string ClassString { get; private set; }
    public string ClassStringStyled { get; private set; }
    public string RenderedRegex { get; }

    public DynamicTokenType(string templateString, string className = null)
    {
        ClassName = className ?? "NewTokenType";
        RenderedRegex = RenderRegexAndRegisterTypeReferences(templateString);
        BuildClassFile(templateString);
    }

    string RenderRegexAndRegisterTypeReferences(string templateString)
    {
        var templateReplacement = Regex.Replace(templateString, @"\@(?<TypeName>\w+)\b", match =>
        {
            var typeName = match.Groups["TypeName"].Value;
            var type = TokenTypeRegistry.NameToType[typeName];

            if (type != null)
            {
                _referencedTypeNames.Add(type.Name);

                if (type.IsAssignableTo(typeof(TokenUnit)) && TokenTypeRegistry.Templates.TryGetValue(type, out var template))
                    return template.RegexString;

                else if (type.IsEnum && TokenTypeRegistry.EnumRegexStrings.TryGetValue(type, out var renderedRegex))
                    return renderedRegex;
            }

            return match.Value;
        });

        return templateReplacement;
    }

    private void BuildClassFile(string templateString)
    {
        var wordBuffer = new List<string>();

        void FlushWordBuffer()
        {
            if (wordBuffer.Count > 0)
            {
                var combinedValue = string.Join(' ', wordBuffer);
                var wordSnippet = $"\"{combinedValue}\"";
                _parameterParts.Add(wordSnippet);
                DynamicSnippets.Add(wordSnippet);
                CombinedParts.Add((combinedValue, isType: false));
                wordBuffer.Clear();
            }
        }

        foreach (var word in templateString.Split(' '))
        {
            if (word.StartsWith("@") && _referencedTypeNames.Contains(word.Substring(1)))
            {
                FlushWordBuffer();
                var typeName = word.Substring(1);
                _propertyParts.Add($"public {typeName} {typeName} {{ get; set; }}");
                _parameterParts.Add($"nameof({typeName})");
                DynamicSnippets.Add(typeName);
                CombinedParts.Add((typeName, isType: true));
            }
            else
            {
                wordBuffer.Add(word);
            }
        }
        FlushWordBuffer();

        // --- plain C# class text ---
        ClassString = 
            $$"""
            namespace MTGPlexer.TokenUnits;

            public class {{ClassName}} : {{nameof(TokenUnit)}}
            {
                public {{ClassName}}() : base ({{string.Join(", ", _parameterParts)}}) { }

                {{string.Join("\r\n    ", _propertyParts)}}
            }
            """;

        // --- HTML‑styled version ---
        var publicKw = "<span class=\"keyword\">public</span>";
        var classKw = "<span class=\"keyword\">class</span>";
        var typeSpan = $"<span class=\"type\">{ClassName}</span>";
        var baseKw = "<span class=\"keyword\">base</span>";
        var tokenUnitSpan = $"<span class=\"type\">{nameof(TokenUnit)}</span>";

        var styledProps = _propertyParts.Select(p =>
        {
            var m = Regex.Match(p, @"public (\w+) (\w+)");
            if (!m.Success) return p;

            var t = m.Groups[1].Value;
            var n = m.Groups[2].Value;
            return
                $"{publicKw} <span class=\"type\">{t}</span> " +
                $"<span class=\"identifier\">{n}</span> {{ " +
                "<span class=\"keyword\">get</span>; " +
                "<span class=\"keyword\">set</span>; }";
        });

        var classStringStyled =
            $$"""
            {{publicKw}} {{classKw}} {{typeSpan}} : {{tokenUnitSpan}}
            {
                {{publicKw}} {{typeSpan}}() : {{baseKw}} ({{string.Join(", ", CombinedParts.Select(x =>
                    x.IsType ? $"<span class=\"keyword\">nameof(</span><span class=\"identifier\">{x.Val}</span><span class=\"keyword\">)</span>"
                    : $"<span class=\"string\">\"{x}\"</span>"))}}) { }

                {{string.Join("\r\n    ", styledProps)}}
            }
            """;

        classStringStyled = classStringStyled.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");
        ClassStringStyled = classStringStyled;
    }

    public string GetRenderedRegexFromTemplate()
    {
        var template = TokenTypeRegistry.GetTemplateFromDynamicTokenType(this);

        return template.Builder.GetMinified();
    }
}