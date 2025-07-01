namespace MTGCardParser;

public class RegexTemplate
{
    protected bool _noSpaces;
    public List<CaptureProp> CaptureProps { get; set; }
    public string RenderedRegex { get; set; }

    //public Dictionary<PropertyInfo, List<PropertyInfo>> AlternatePropSets { get; set; } = new();
    //public Dictionary<PropertyInfo, IPropRegexSegment> PropRegexSegments { get; set; } = new();

    public List<IRegexSegment> RegexSegments { get; set; } = new();

    public List<IPropRegexSegment> PropCaptureSegments => 
        RegexSegments
        .OfType<IPropRegexSegment>()
        .ToList();

    public RegexTemplate()
    {
    }

    public RegexTemplate(RegexTemplate source)
    {
        //PropRegexSegments = source.PropRegexSegments.ToDictionary();
        RegexSegments = source.RegexSegments.ToList();
        RenderedRegex = source.RenderedRegex;
    }
}

public class RegexTemplate<T> : RegexTemplate
{
    public RegexTemplate(params object[] templateSnippets)
    {
        CaptureProps = GetPropertiesForCapture();
        _noSpaces = typeof(T).GetCustomAttribute<NoSpacesAttribute>() is not null;

        // Combines single snippets and array snippets
        //List<string> unpackedTemplateSnippets = new();

        //foreach (var snippetObj in templateSnippets)
        //{
        //    if (snippetObj is string snippetString)
        //        unpackedTemplateSnippets.Add(snippetString);
        //    else if (snippetObj is CaptureAlternatives captureAlternatives)
        //    {
        //        List<PropertyInfo> alternatePropSet = new();
        //        
        //        foreach (var snippetArrayItem in captureAlternatives.Names)
        //        {
        //            var matchingProp = captureProps.FirstOrDefault(x => x.Name == snippetArrayItem);
        //
        //            if (matchingProp is null)
        //                throw new Exception($"Each snippet passed in an array must match a property, but found no property name {snippetArrayItem}");
        //
        //            alternatePropSet.Add(matchingProp);
        //            unpackedTemplateSnippets.Add(snippetArrayItem);
        //        }
        //
        //        // Each prop in the set gets its own key pointing to the List
        //        foreach (var alternateProp in alternatePropSet)
        //            AlternatePropSets[alternateProp] = alternatePropSet;
        //    }
        //    else
        //        throw new Exception($"Snippet must only be of type string or CaptureAlternatives");
        //}
        //
        //for (int i = 0; i < unpackedTemplateSnippets.Count; i++)
        //{
        //    string snippet = unpackedTemplateSnippets[i];
        //    var matchingProp = captureProps.FirstOrDefault(x => x.Name == snippet);
        //
        //    if (matchingProp != null)
        //    {
        //        IRegexSegment regexSegment;
        //
        //        var underlyingType = Nullable.GetUnderlyingType(matchingProp.PropertyType) ?? matchingProp.PropertyType;
        //
        //        if (underlyingType.IsEnum)
        //            regexSegment = new EnumCaptureGroup(underlyingType, matchingProp);
        //        else if (underlyingType.IsAssignableTo(typeof(ITokenCapture)))
        //            regexSegment = TokenCaptureSubPropertyToCaptureGroup(matchingProp);
        //        else
        //            regexSegment = PropertyToCaptureGroup(matchingProp);
        //
        //        RegexSegments.Add(regexSegment);
        //        PropRegexSegments[matchingProp] = regexSegment;
        //    }
        //    else
        //        RegexSegments.Add(new TextRegexSegment(snippet));
        //}

        foreach (var snippetObj in templateSnippets)
        {
            IRegexSegment resolvedSegment;

            if (snippetObj is string snippetString)
                resolvedSegment = ResolveSnippetToRegexSegment(snippetString);

            else if (snippetObj is CaptureAlternatives captureAlternatives)
            {
                List<TokenCaptureSegment> alternativeTokenCaptureSegments = new();

                foreach (var alternative in captureAlternatives.Names)
                {
                    var resolvedAlternative = (TokenCaptureSegment)ResolveSnippetToRegexSegment(alternative, forceResolveTokenUnit: true);
                    alternativeTokenCaptureSegments.Add(resolvedAlternative);
                }

                resolvedSegment = new TokenCaptureAlternativeSet(alternativeTokenCaptureSegments);
            }
            else
                throw new Exception($"Each snippet must be of type string or CaptureAlternatives");

            RegexSegments.Add(resolvedSegment);
        }

        //int currentAlternateIndex = -1;
        //List<PropertyInfo> currentAlternateSet = null;
        //bool isLastAlternate = false;
        //
        ////for (int i = 0; i < RegexSegments.Count; i++)
        //{
        //    var segment = RegexSegments[i];
        //    var segmentString = segment.RegexString;
        //
        //    if (segment is IPropRegexSegment propRegexSegment && AlternatePropSets.TryGetValue(propRegexSegment.Prop, out currentAlternateSet))
        //    {
        //        currentAlternateIndex = currentAlternateSet.IndexOf(propRegexSegment.Prop);
        //
        //        if (currentAlternateIndex == 0)
        //            // Begin alternate capture
        //            RenderedRegex += "(";
        //
        //        isLastAlternate = currentAlternateIndex == currentAlternateSet.Count - 1;
        //    }
        //    else
        //    {
        //        currentAlternateIndex = -1;
        //        currentAlternateSet = null;
        //    }
        //
        //    RenderedRegex += segmentString;
        //    var shouldAddAltPipe = currentAlternateSet != null && !isLastAlternate;
        //
        //    if (shouldAddAltPipe)
        //        RenderedRegex += "|";
        //
        //    if (isLastAlternate)
        //    {
        //        // End alternate capture
        //        RenderedRegex += ")";
        //        isLastAlternate = false;
        //    }
        //
        //    var shouldAddSpace =
        //        !_noSpaces
        //        && (currentAlternateIndex == -1 || isLastAlternate)
        //        && i < RegexSegments.Count - 1
        //        && !(segment is TokenCaptureSegment capGroup && capGroup.CapturePropType == CapturePropType.Bool)
        //        && !TerminalPunctuation.Contains(segmentString);
        //
        //    if (shouldAddSpace)
        //        RenderedRegex += " ";
        //}

        for (int i = 0; i < RegexSegments.Count; i++)
        {
            var segment = RegexSegments[i]; 
            RenderedRegex += segment.RegexString;

            var shouldAddSpace =
                !_noSpaces
                && i < RegexSegments.Count - 1
                && !(segment is PropCaptureSegment propCap && propCap.CaptureProp.CapturePropType == CapturePropType.Bool)
                && !TerminalPunctuation.Contains(segment.RegexString);

            if (shouldAddSpace)
                RenderedRegex += " ";
        }

        // We don't need word boundaries where there are spaces (this step just improves regex human readability)
        RenderedRegex = RenderedRegex.Replace(@"\b \b", " ");
    }

    static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    CaptureProp GetMatchingProp(string propName, bool isRequiredToExistOnType = false)
    {
        var matchingProp = CaptureProps.FirstOrDefault(x => x.Name == propName);

        if (isRequiredToExistOnType && matchingProp is null)
            throw new Exception($"Property {propName} is required, but not found on type '{typeof(T).Name}'");

        return matchingProp;
    }

    //IRegexSegment ResolvePropToRegexSegment(PropertyInfo prop)
    //{
    //    IRegexSegment resolvedSegment;
    //
    //    var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
    //
    //    if (underlyingType.IsEnum)
    //        resolvedSegment = new EnumCaptureGroup(underlyingType, prop);
    //    else if (underlyingType.IsAssignableTo(typeof(ITokenizedCard)))
    //        resolvedSegment = TokenCaptureSubPropertyToCaptureGroup(prop);
    //    else
    //        resolvedSegment = PropertyToCaptureGroup(prop);
    //
    //    return resolvedSegment;
    //}

    IRegexSegment ResolveSnippetToRegexSegment(string templateSnippet, bool forceResolveTokenUnit = false)
    {
        var matchingProp = GetMatchingProp(templateSnippet, isRequiredToExistOnType: forceResolveTokenUnit);

        if (matchingProp is not null)
        {
            var isTokenUnitType = matchingProp.UnderlyingType.IsAssignableTo(typeof(ITokenUnit));

            if (forceResolveTokenUnit && matchingProp.CapturePropType != CapturePropType.TokenUnit)
                throw new Exception($"Prop type {matchingProp.UnderlyingType.Name} is required to implement ({nameof(ITokenUnit)})");

            return matchingProp.CapturePropType switch
            {
                CapturePropType.TokenUnit => new TokenCaptureSegment(matchingProp),
                CapturePropType.Enum => new EnumCaptureSegment(matchingProp),
                _ => new PropCaptureSegment(matchingProp),
            };
        }
        else
            return new TextSegment(templateSnippet);
    }

    //TokenCaptureSegment TokenCaptureSubPropertyToCaptureGroup(CaptureProp subTokenCaptureProp)
    //{
    //    var instanceOfPropType = (ITokenUnit)Activator.CreateInstance(subTokenCaptureProp.Prop.PropertyType);
    //    return new TokenCaptureSegment(subTokenCaptureProp, instanceOfPropType.RegexTemplate.RenderedRegex);
    //}
    //
    //PropCaptureSegment PropertyToCaptureGroup(CaptureProp captureProp)
    //{
    //    var patterns = regexPatternAttribute?.Patterns ?? [prop.Name];
    //    var groupIsOptional = prop.PropertyType == typeof(bool);
    //
    //    return new PropCaptureSegment(prop, regexPatternAttribute.Patterns, captureGroupType, new());
    //}

    List<CaptureProp> GetPropertiesForCapture() =>
         typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x => 
            (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum 
             || x.PropertyType == typeof(bool) 
             || x.PropertyType == typeof(CapturedTextSegment) 
             || x.PropertyType.IsAssignableTo(typeof(ITokenUnit)))
        .Select(x => new CaptureProp(x))
        .ToList();
}

