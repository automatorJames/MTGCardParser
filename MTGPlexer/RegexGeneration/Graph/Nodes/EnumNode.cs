namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumNode : ScalarContainerNode
{
    public EnumNode(RegexNode parentNode, TypeNavigation navigation) : base(parentNode, navigation)
    {
    }

    protected override List<RegexNode> GetChildNodes()
    {
        List<RegexNode> children = [];
        var enumType = Navigation.UnderlyingType;
        var scalarValues = Enum.GetValues(enumType).Cast<object>().ToList();

        for (int i = 0; i < scalarValues.Count; i++)
        {
            var scalarValue = scalarValues[i];
            var valueAsString = scalarValue.ToString();
            bool isFirst = i == 0;

            if (Navigation.Patterns is string[] patterns && patterns.Length > 0)
            {
                if (patterns.Length == 1)
                    // If there's only one "synonym", it's really just an alias for the scalar value
                    children.Add(new ScalarNode(
                        parentNode: this, 
                        name: valueAsString, 
                        scalarValue: scalarValue, 
                        regex: patterns[0], 
                        isFirst: isFirst));
                else
                    // If there are two or more, they're true synonyms
                    children.Add(new ScalarSynonymSet(
                        parentNode: this, 
                        name: valueAsString, 
                        scalarValue: scalarValue, 
                        scalarSynonyms: patterns, 
                        isFirst: isFirst));
            }
            else
                // This is a single scalar node whose regex is a formatted version of the enum member
                children.Add(new ScalarNode(
                    parentNode: this, 
                    name: valueAsString, 
                    scalarValue: scalarValue, 
                    regex: valueAsString.ToFriendlyCase(), 
                    isFirst: isFirst));
        }

        return children;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoinedAlternating(this, Children);
        collector.Append(GroupCloseBrick);
    }

    public override object GetValueSingle(Capture capture)
    {
        return Children
            .OfType<INamedScalarValue>()
            .FirstOrDefault(x => x.Name == capture.Value)
            .ScalarValue
            ?? throw new Exception($"Found no matching values for enum '{Navigation.UnderlyingType.Name}' from match string '{capture.Value}'");
    }
}
