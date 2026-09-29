namespace Glyphotype.Tests.Grammar;

// The test grammar's word lists. Deliberately small and non-overlapping, so every corpus sentence has
// exactly one intended parse even though every test glyph competes for every line.

public enum Animal
{
    Dog,
    Cat,
    Bird,

    /// <summary>Synonyms via <see cref="RegexPatternAttribute"/> on an enum member.</summary>
    [RegexPattern("horse", "pony")] Horse,
}

public enum Place
{
    Kitchen,
    Garden,
    Barn,

    /// <summary>No pattern of its own: friendly-cased from the member name to "living room".</summary>
    LivingRoom,
}

public enum Food
{
    Fish,
    Milk,
    Cheese,
    Bread,

    /// <summary>Its pattern contains an earlier member's ("fish"), so hydration must match a captured value whole.</summary>
    FishSticks,
}

/// <summary>Each member also matches its plural ("apples", "cherries").</summary>
[OptionalPlural]
public enum Fruit
{
    Apple,
    Pear,
    Cherry,
}

public enum Weekday
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
}

public enum Holiday
{
    Christmas,
    [RegexPattern("new year's day")] NewYearsDay,
}

public enum Person
{
    Teacher,
    Baker,
    Doctor,
    [RegexPattern("mail carrier", "mailman")] MailCarrier,
}

public enum Building
{
    Shop,
    School,
    Clinic,
}

public enum Trait
{
    Big,
    Small,
    Old,
    Young,
    Friendly,
    Sleepy,
}

public enum Ingredient
{
    Flour,
    Sugar,
    Eggs,
    Butter,

    /// <summary>Its pattern starts with an earlier member's ("butter"), which must not shadow it.</summary>
    Buttermilk,
}

public enum Weather
{
    Rains,
    Snows,
    [RegexPattern("is sunny")] IsSunny,
}

public enum Meridiem
{
    Am,
    Pm,
}

public enum Intensifier
{
    Very,
    Really,
}

public enum ClockSound
{
    Tick,
    Tock,
}

public enum TimeOfDay
{
    Morning,
    Evening,
}
