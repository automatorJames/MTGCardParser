namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public class RegexLineCollector
{
    Type _topLevelType;
    int _nextUnnamedCaptureGroupId;
    List<RegexTemplateLine> _lines = [];
    Stack<object> _captureGroupStack = [];
    int _indentation;

    // The key "-1" represents the top level (i.e. the class, not within a capture group)
    Dictionary<object, SpaceDisposition> _spaceIsRequiredBeforeNextElementAtLevel;

    public RegexLineCollector(Type topLevelType)
    {
        _topLevelType = topLevelType;
        var topLevelSpaceDiposition = topLevelType.IsDefined(typeof(NoSpacesAttribute)) ? SpaceDisposition.NeverAddSpace : SpaceDisposition.DontAddSpaceBeforeNextItem;
        _spaceIsRequiredBeforeNextElementAtLevel = new Dictionary<object, SpaceDisposition> { [-1] = topLevelSpaceDiposition };
    }

    public void OpenGroup(RegexPropInfo captureGroup = null, bool neverAddSpacesToGroupMembers = false)
    {
        AddPrecedingSpaceAndBlankIfApplicable();
        var groupKey = (object)captureGroup ?? (object)_nextUnnamedCaptureGroupId++;
        _captureGroupStack.Push(groupKey);

        if (groupKey is RegexPropInfo prop && prop.BaseType.IsDefined(typeof(NoSpacesAttribute)))
            neverAddSpacesToGroupMembers = true;

        _spaceIsRequiredBeforeNextElementAtLevel[groupKey] = neverAddSpacesToGroupMembers ? SpaceDisposition.NeverAddSpace : SpaceDisposition.DontAddSpaceBeforeNextItem;

        if (captureGroup != null)
            _lines.Add(new NamedGroupOpen(captureGroup.Name, GetFlatNamePath(), _indentation));
        else
            _lines.Add(new GroupOpen(GetFlatNamePath(), _indentation));
         
        _indentation++;
    }

    public void CloseGroup(GroupQuantifier? quantifier = null)
    {
        _indentation--;
        _lines.Add(new GroupClose(GetFlatNamePath(), _indentation, quantifier));

        // Pop the current group name (or null placeholder)
        _captureGroupStack.Pop();
    }


    public void AddTextLine(string text)
    {
        AddPrecedingSpaceAndBlankIfApplicable();
        _lines.Add(new TextLine(text, GetFlatNamePath(), _indentation));
    }

    public void AddAlternateValues(IEnumerable<string> alternatives)
    {
        bool isFirstAlternation = true;

        foreach (var alternative in alternatives)
        {
            var alternateValue = new AlternateValue(alternative, GetFlatNamePath(), _indentation, isFirstAlternation);
            _lines.Add(alternateValue);
            isFirstAlternation = false;
        }
    }

    public void AddGroupAlternativePipe() => _lines.Add(new GroupAlternativePipe(GetFlatNamePath(), _indentation));

    void AddPrecedingSpaceAndBlankIfApplicable()
    {
        var lastNamedCaptureGroup = _captureGroupStack.OfType<RegexPropInfo>().LastOrDefault();

        if (lastNamedCaptureGroup != null)
        {
            var groupSpaceDisposition = _spaceIsRequiredBeforeNextElementAtLevel[lastNamedCaptureGroup];

            if (groupSpaceDisposition == SpaceDisposition.AddSpaceBeforeNextItem)
                AddPrecedingSpaceAndBlank();
            else if (groupSpaceDisposition != SpaceDisposition.NeverAddSpace)
                _spaceIsRequiredBeforeNextElementAtLevel[lastNamedCaptureGroup] = SpaceDisposition.AddSpaceBeforeNextItem;

        }
        else
        {
            // -1 represents the top level
            var topLevelDisposition = _spaceIsRequiredBeforeNextElementAtLevel[-1];

             if (topLevelDisposition == SpaceDisposition.AddSpaceBeforeNextItem) 
                AddPrecedingSpaceAndBlank();
            else if (topLevelDisposition != SpaceDisposition.NeverAddSpace)
                _spaceIsRequiredBeforeNextElementAtLevel[-1] = SpaceDisposition.AddSpaceBeforeNextItem;
        } 
        
        // private helper
        void AddPrecedingSpaceAndBlank()
        {
            _lines.Add(new SpaceLine(GetFlatNamePath(), _indentation));
            //_lines.Add(new BlankLine(GetFlatNamePath()));
        }
    }

    public List<RegexTemplateLine> Finalize()
    {
        if (!_topLevelType.IsDefined(typeof(NoBoundaryAttribute)))
        {
            _lines.Insert(0, new NegativeLookbehindBoundary());
            _lines.Add(new NegativeLookaheadBoundary());
        }

        return _lines;
    }

    /// <summary>
    /// Get the current dot-navigaiton name path, which exclude any null name parts (representing unnamed parentheses groups).
    /// </summary>
    string GetFlatNamePath() => string.Join("_", _captureGroupStack.OfType<RegexPropInfo>().Where(x => x != null).Select(x => x.Name));
}

public enum SpaceDisposition
{
    NeverAddSpace,
    DontAddSpaceBeforeNextItem,
    AddSpaceBeforeNextItem,
}