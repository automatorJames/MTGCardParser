namespace Glyphotype.Tests.Grammar;

// Glyphs within glyphs, optional parts, and repeated parts.

[Dependent]
public class OnDay : Glyph
{
    public override Nib[] Nibs => ["on", Prop(Weekday)];

    public Weekday Weekday { get; set; }
}

/// <summary>An <c>int</c> whose <see cref="RegexPatternAttribute"/> overrides the default pattern: an hour is one or two digits.</summary>
[Dependent]
public class AtTime : Glyph
{
    public override Nib[] Nibs => ["at", Prop(Hour), Prop(Meridiem)];

    [RegexPattern(@"\d{1,2}")] public int Hour { get; set; }
    public Meridiem Meridiem { get; set; }
}

/// <summary>A required nested glyph, and an <see cref="OptionalAttribute"/> one.</summary>
public class PersonComes : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "comes", Prop(Day), Prop(Time)];

    public Person Person { get; set; }
    public OnDay Day { get; set; }
    [Optional] public AtTime Time { get; set; }
}

/// <summary><see cref="OptionalOf{T}"/>: optionality expressed by the property's type.</summary>
public class MailArrives : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "brings the mail", Prop(Day)];

    public Person Person { get; set; }
    public OptionalOf<OnDay> Day { get; set; }
}

// Repetition is expressed with the internal primitives, never a List<> (which validation refuses).

/// <summary>Zero or more of a word: an <see cref="OptionalAttribute"/> space-joined <see cref="CompoundOf{T}"/>.</summary>
public class AnimalSings : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "sings", Prop(Intensifiers), "loudly"];

    public Animal Animal { get; set; }
    [Optional, JoinedBy(Joiner.Space)] public CompoundOf<Intensifier> Intensifiers { get; set; }
}

/// <summary>One or more of a word: a required space-joined <see cref="CompoundOf{T}"/>.</summary>
public class ClockGoes : Glyph
{
    public override Nib[] Nibs => ["the clock goes", Prop(Sounds)];

    [JoinedBy(Joiner.Space)] public CompoundOf<ClockSound> Sounds { get; set; }
}
