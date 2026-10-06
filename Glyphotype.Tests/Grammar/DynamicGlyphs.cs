namespace Glyphotype.Tests.Grammar;

// DynamicGlyph: a property resolved at match time by tokenizing its captured text.

/// <summary>An unfiltered <see cref="DynamicGlyph"/>: resolves to any glyph that consumes the rest of the clause.</summary>
public class IfWeather : Glyph
{
    public override Nib[] Nibs => ["if it", Prop(Weather), ",", Prop(Outcome)];

    public Weather Weather { get; set; }
    public DynamicGlyph Outcome { get; set; }
}

/// <summary>Matches the start of what <see cref="CatDozesInTheSun"/> does, with a longer regex, so the Tokenizer tries it first.</summary>
public class PetDozes : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), Alt("dozes", "snoozes", "slumbers", "drowses", "naps")];

    public Animal Animal { get; set; }
}

/// <summary>What a dynamic resolves "the cat dozes in the sun" to: the glyph that covers all of it, not <see cref="PetDozes"/>, which covers only its start.</summary>
public class CatDozesInTheSun : Glyph
{
    public override Nib[] Nibs => ["the cat dozes in the sun"];
}

/// <summary>Marker for the glyphs <see cref="EveryWeekday"/>'s <see cref="TypeFilterAttribute"/> accepts.</summary>
public interface IChore
{
}

/// <summary>A <see cref="TypeFilterAttribute"/>-scoped <see cref="DynamicGlyph"/>: resolves only to an <see cref="IChore"/>.</summary>
public class EveryWeekday : Glyph
{
    public override Nib[] Nibs => ["every", Prop(Weekday), ",", Prop(Chore)];

    public Weekday Weekday { get; set; }
    [TypeFilter(typeof(IChore))] public DynamicGlyph Chore { get; set; }
}

/// <summary>A chore that's also top-level.</summary>
public class WeWaterThePlants : Glyph, IChore
{
    public override Nib[] Nibs => ["we water the plants"];
}

/// <summary>A dependent chore, matched by its friendly-cased type name: only reachable through a dynamic.</summary>
[Dependent]
public class WeSweepTheFloor : Glyph, IChore
{
}

/// <summary>A dependent chore matched by a class-level <see cref="RegexPatternAttribute"/>.</summary>
[Dependent]
[RegexPattern("we (wash|dry) the dishes")]
public class DoTheDishes : Glyph, IChore
{
}
