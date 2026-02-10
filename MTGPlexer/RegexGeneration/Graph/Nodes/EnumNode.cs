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

            var scalarSynonyms = enumType
                .GetField(valueAsString)
                .GetCustomAttribute<RegexPatternAttribute>()?
                .Patterns ?? [];

            if (scalarSynonyms.Length > 0)
            {
                if (scalarSynonyms.Length == 1)
                {
                    // If there's only one "synonym", it's really just an alias for the scalar value
                    children.Add(new ScalarNode(this, scalarValue, scalarSynonyms[0], isFirst: isFirst));
                }
                else
                {
                    // If there are two or more, they're true synonyms
                    TypeNavigation typeNavigation = new(typeof(Enum), name: valueAsString);
                    children.Add(new ScalarSynonymSet(this, typeNavigation, scalarValue, scalarSynonyms, isFirst));
                }
            }
            else
                children.Add(new ScalarNode(this, scalarValue, valueAsString, isFirst: isFirst));
        }

        return children;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(Joiner.Pipe));
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
