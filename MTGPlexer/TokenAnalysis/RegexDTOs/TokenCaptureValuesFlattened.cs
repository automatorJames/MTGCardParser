namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TokenCaptureValuesFlattened
{
    public List<PropPathValAsString> PropPathValAsStrings { get; }

    public TokenCaptureValuesFlattened(TokenUnit hydratedTokenUnit)
    {
        List<PropPathValAsString> combinedList = [];

        foreach (var indexedPropertyCapture in hydratedTokenUnit.IndexedPropertyCaptures)
        {
            var rootPropName = indexedPropertyCapture.RegexPropInfo.FriendlyPropName;
            var rootValue = indexedPropertyCapture.Value;
            var propVals = GetFlattenedValuesAsStringsRecursive(rootPropName, rootValue);
            combinedList.AddRange(propVals);
        }

        PropPathValAsStrings = combinedList;
    }

    List<PropPathValAsString> GetFlattenedValuesAsStringsRecursive(string currentPropPath, object currentValue, List<PropPathValAsString> runningList = null)
    {
        runningList ??= [];

        if (currentValue == null)
            return runningList;

        if (currentValue is TokenUnit childTokenUnit)
        {
            var props = childTokenUnit.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var prop in props)
            {
                var childVal = prop.GetValue(currentValue);
                var childPropPath = currentPropPath.Colon(prop.Name.ToFriendlyCase());
                GetFlattenedValuesAsStringsRecursive(childPropPath, childVal, runningList);
            }
        }
        else if (currentValue is TokenUnitOneOf tokenUnitOneOf)
        {
            var props = tokenUnitOneOf.GetType().GetProperties().Where(x => x.PropertyType == typeof(TokenUnit));

            foreach (var prop in props)
            {
                var childVal = prop.GetValue(currentValue);

                if (childVal != null)
                {
                    var childPropPath = currentPropPath.Colon(prop.Name.ToFriendlyCase());
                    GetFlattenedValuesAsStringsRecursive(childPropPath, childVal, runningList);
                    break;
                }
            }
        }
        else if (currentValue is TokenUnitDistilled tokenUnitDistilled)
        {
            foreach (var placeholderPropItem in tokenUnitDistilled.DistilledValues)
            {
                foreach (var distilledItem in placeholderPropItem.Value)
                {
                    var childPropPath = currentPropPath.Colon(distilledItem.Key.Name.ToFriendlyCase());
                    var childValString = distilledItem.Value.ToString();
                    runningList.Add(new PropPathValAsString(childPropPath, childValString));
                }
            }
        }
        else
            runningList.Add(new PropPathValAsString(currentPropPath, currentValue.ToString()));

        return runningList;
    }
}

