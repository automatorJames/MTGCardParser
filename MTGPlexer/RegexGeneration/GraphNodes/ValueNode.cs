namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class ValueNode : Node
{
    protected ValueNode(Node parentNode, string name)
    : base(parentNode, name)
    {
    }

    /// <summary>
    /// Processes a collection of captures to produce a property value. 
    /// Default behavior: If exactly one capture exists, it forwards to <see cref="GetValue(Capture)". 
    /// If multiple captures exist, it throws an exception requiring an override.</para>
    /// </summary>
    public virtual object GetValue(Capture[] captures)
    {
        if (captures.Length == 1)
            return GetValue(captures[0]);

        if (captures.Length > 1)
            throw new Exception($"'{this.GetType().Name}' received {captures.Length} captures, but it does not override 'GetPropertyValue(Capture[] captures)' to handle multiple values.");

        // Note: This case should be unreachable.
        throw new Exception($"'{this.GetType().Name}' received zero captures. This indicates an upstream failure in the match-verification logic.");
    }

    /// <summary>
    /// Processes a single capture to produce a property value. 
    /// Override this for simple nodes that expect a 1:1 mapping between a token capture and a property value.
    /// </summary>
    public virtual object GetValue(Capture capture)
    {
        throw new Exception($"'{this.GetType().Name}' must override either 'GetPropertyValue(Capture[] captures)' or 'GetPropertyValue(Capture capture)'.");
    }
}
