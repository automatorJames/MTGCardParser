namespace Glyphotype.Tests.Grammar;

// Parts written with no space between them: a CompoundOf joined by nothing, braced one-of items, and a
// Joiner.None override.

public enum RomanLetter
{
    I,
    V,
    X,
}

/// <summary>A <see cref="CompoundOf{T}"/> joined by nothing: "xiv" is <c>[X, I, V]</c>.</summary>
public class ReadsChapter : Glyph
{
    public override Nib[] Nibs => ["the teacher reads chapter", Prop(Chapter)];

    [JoinedBy(Joiner.None)] public CompoundOf<RomanLetter> Chapter { get; set; }
}

public enum ModifierKey
{
    Ctrl,
    Alt,
    Shift,
}

public enum LetterKey
{
    C,
    V,
    Z,
}

/// <summary>A <see cref="GlyphOneOf"/> with literal text around its alternatives: one bracketed key, e.g. "[ctrl]", "[c]" or "[2]".</summary>
[Dependent]
public class KeyCap : GlyphOneOf
{
    public override Nib[] Nibs => ["[", Prop(Modifier), Prop(Letter), Prop(Digit), "]"];

    public ModifierKey? Modifier { get; set; }
    public LetterKey? Letter { get; set; }
    public int? Digit { get; set; }
}

public enum EditAction
{
    Copy,
    Paste,
    Undo,
    Zoom,
}

/// <summary>A fused cluster of braced items: "[ctrl][shift][z]".</summary>
public class PressToAct : Glyph
{
    public override Nib[] Nibs => ["press", Prop(Keys), "to", Prop(Action)];

    [JoinedBy(Joiner.None)] public CompoundOf<KeyCap> Keys { get; set; }
    public EditAction Action { get; set; }
}

/// <summary>A <see cref="Joiner.None"/> override: "$" and the amount are written with no space between them.</summary>
[Dependent]
public class Price : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["$", Prop(Dollars)];

    public int Dollars { get; set; }
}

public class FruitCosts : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Fruit), Alt("costs", "cost"), Prop(Price)];

    public Fruit Fruit { get; set; }
    public Price Price { get; set; }
}
