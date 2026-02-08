namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : GroupNode
{
    protected override RegexBrick GroupOpenBrick => new(this, $"?<{FullyQualifiedName}>", FullyQualifiedName);
    public string FullyQualifiedName { get; }

    public NamedGroupNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        FullyQualifiedName = string.Join("_", Lineage.Where(x => !x.IsCollapsible));
    }

    //public bool SetPropertyValue(CaptureContext captureContext, TokenUnit parent)
    //{
    //    if (ConcreteProperty == null)
    //        throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");
    //
    //    var value = GetValueAndSetHydrationInfo(captureContext);
    //
    //    if (value == null && AbortIfSetPropertyToNull)
    //        return false;
    //
    //    ConcreteProperty.SetValue(parent, value);
    //
    //    return true;
    //}
}
