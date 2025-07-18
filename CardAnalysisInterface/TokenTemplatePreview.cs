using Microsoft.AspNetCore.Components;
using System.Linq;

namespace CardAnalysisInterface;

public record TokenTemplatePreview
{
    List<string> _referencedTypeNames = [];

    public string ClassName { get; }
    public string ClassFile { get; private set; }
    public MarkupString ClassFileStyled { get; private set; }
    public string RenderedRegex { get; }

    public TokenTemplatePreview(string templateString, string className = null)
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
                    return template.RenderedRegexString;

                else if (type.IsEnum && TokenTypeRegistry.EnumRegexStrings.TryGetValue(type, out var renderedRegex))
                    return renderedRegex;
            }

            return match.Value;
        });

        return templateReplacement;
    }

    private void BuildClassFile(string templateString)
    {
        var parameterParts = new List<string>();
        var propertyParts = new List<string>();
        var wordBuffer = new List<string>();

        void FlushWordBuffer()
        {
            if (wordBuffer.Count > 0)
            {
                parameterParts.Add($"\"{string.Join(' ', wordBuffer)}\"");
                wordBuffer.Clear();
            }
        }

        foreach (var word in templateString.Split(' '))
        {
            if (word.StartsWith("@") && _referencedTypeNames.Contains(word.Substring(1)))
            {
                FlushWordBuffer();
                var typeName = word.Substring(1);
                parameterParts.Add($"nameof({typeName})");
                propertyParts.Add($"public {typeName} {typeName} {{ get; set; }}");
            }
            else
            {
                wordBuffer.Add(word);
            }
        }
        FlushWordBuffer();

        // --- plain C# class text ---
        ClassFile = 
            $$"""
            public class {{ClassName}} : {{nameof(TokenUnit)}}
            {
                public {{ClassName}}() : base ({{string.Join(", ", parameterParts)}}) { }

                {{string.Join("\r\n    ", propertyParts)}}
            }
            """;

        // --- HTML‑styled version ---
        var publicKw = "<span class=\"keyword\">public</span>";
        var classKw = "<span class=\"keyword\">class</span>";
        var typeSpan = $"<span class=\"type\">{ClassName}</span>";
        var baseKw = "<span class=\"keyword\">base</span>";
        var tokenUnitSpan = $"<span class=\"type\">{nameof(TokenUnit)}</span>";

        var styledProps = propertyParts.Select(p =>
        {
            var m = Regex.Match(p, @"public (\w+) (\w+)");
            if (!m.Success) return p;

            var t = m.Groups[1].Value;
            var n = m.Groups[2].Value;
            return
                $"{publicKw} <span class=\"type\">{t}</span> " +
                $"<span class=\"identifier\">{n}</span> {{ " +
                "<span class=\"keyword\">get</span>; " +
                "<span class=\"keyword\">set</span>; }}";
        });

        var classFileStyledText =
            $$"""
            {{publicKw}} {{classKw}} {{typeSpan}} : {{tokenUnitSpan}}
            {
                {{publicKw}} {{typeSpan}}() : {{baseKw}} ({{string.Join(", ", parameterParts.Select(p =>
                p.StartsWith("nameof")
                  ? $"<span class=\"identifier\">{p}</span>"
                  : $"<span class=\"string\">{p}</span>"))}}) { }

                {{string.Join("\r\n    ", styledProps)}}
            }
            """;

        classFileStyledText = classFileStyledText.Replace("  ", "&nbsp;&nbsp;").Replace("\r\n", "<br>");
        ClassFileStyled = new MarkupString(classFileStyledText);
    }
}