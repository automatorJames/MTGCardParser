namespace MTGCardParser.Enums;

public enum Quantity
{
    [RegPat("none", "zero", "0")]
    Zero = 0,

    [RegPat("one", "1")]
    One = 1,

    [RegPat("two", "2")]
    Two = 2,

    [RegPat("three", "3")]
    Three = 3,

    [RegPat("four", "4")]
    Four = 4,

    [RegPat("five", "5")]
    Five = 5,

    [RegPat("six", "6")]
    Six = 6,

    [RegPat("seven", "7")]
    Seven = 7,

    [RegPat("eight", "8")]
    Eight = 8,

    [RegPat("nine", "9")]
    Nine = 9,

    [RegPat("ten", "10")]
    Ten = 10
}

public enum CardType
{
    Artifact,
    Creature,
    Enchantment,
    Instant,
    Land,
    Planeswalker,
    Sorcery,
    Battle,
    Tribal
}

