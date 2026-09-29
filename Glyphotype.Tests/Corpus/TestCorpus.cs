namespace Glyphotype.Tests.Corpus;

/// <summary>
/// The dummy corpus and what each document must tokenize into, written in <see cref="GlyphSignature"/>
/// notation. Every document is its own test case (see <see cref="CorpusCaptureTests"/>).
/// <para>
/// Sections follow the grammar files. Each pairs positive documents with negative ones: text that looks
/// close to a match but must stay unmatched, guarding against a glyph starting to over-match (an unescaped
/// period, a joiner accepting the wrong separator, a word matching inside a longer word, and so on).
/// </para>
/// <para>
/// All test glyphs compete for every line, so a new glyph can legitimately change an existing expectation.
/// When that happens, prefer rewording the new glyph's sentences over weakening the old expectation.
/// </para>
/// </summary>
public static class TestCorpus
{
    public static readonly IReadOnlyList<TestDocument> Documents =
    [
        .. Section("Literals, Alt() and enums",
            Doc("the dog sleeps in the kitchen.", "AnimalRests{Animal=Dog, Place=Kitchen} ."),
            Doc("the cat naps in the living room.", "AnimalRests{Animal=Cat, Place=LivingRoom} ."),
            Doc("the pony sleeps in the barn.", "AnimalRests{Animal=Horse, Place=Barn} ."),
            Doc("The Dog sleeps in the Garden.", "AnimalRests{Animal=Dog, Place=Garden} ."),
            Doc("the dog sleeps in the garage.", "«the dog sleeps in the garage» ."),
            Doc("the dog sleeps in the kitchenette.", "«the dog sleeps in the kitchenette» ."),
            Doc("the dog snores in the kitchen.", "«the dog snores in the kitchen» .")),

        .. Section("Opt()",
            Doc("the cat eats fish.", "AnimalEats{Animal=Cat, Food=Fish} ."),
            Doc("the cat eats some fish.", "AnimalEats{Animal=Cat, Food=Fish} ."),
            Doc("the cat eats fish sticks.", "AnimalEats{Animal=Cat, Food=FishSticks} ."),
            Doc("the cat eats a fish.", "«the cat eats a fish» .")),

        .. Section("bool properties",
            Doc("the cat walks quietly to the garden.", "AnimalWalks{Animal=Cat, Quietly=True, Place=Garden} ."),
            Doc("the cat walks to the garden.", "AnimalWalks{Animal=Cat, Place=Garden} ."),
            Doc("the cat walks loudly to the garden.", "«the cat walks loudly to the garden» .")),

        .. Section("int properties and [OptionalPlural] enums",
            Doc("there are 3 apples in the bowl.", "FruitInBowl{Count=3, Fruit=Apple} ."),
            Doc("there is 1 pear in the bowl.", "FruitInBowl{Count=1, Fruit=Pear} ."),
            Doc("there are 12 cherries in the bowl.", "FruitInBowl{Count=12, Fruit=Cherry} ."),
            Doc("there are three apples in the bowl.", "«there are three apples in the bowl» .")),

        .. Section("Literal nib text",
            Doc("the teacher asks what is 2+2?", "AsksARiddle"),
            Doc("the teacher asks what is 22?", "«the teacher asks what is 22?»"),
            Doc("the teacher asks what is 2+2", "«the teacher asks what is 2+2»")),

        .. Section("Plural()",
            Doc("feed all the dogs before noon.", "FeedAll{Animal=Dog} ."),
            Doc("feed all the horses before noon.", "FeedAll{Animal=Horse} ."),
            Doc("feed all the dog s before noon.", "«feed all the dog s before noon» .")),

        .. Section("Nested glyphs and [Optional]",
            Doc("the doctor comes on monday.", "PersonComes{Person=Doctor, Day=OnDay{Weekday=Monday}} ."),
            Doc("the doctor comes on monday at 9 am.", "PersonComes{Person=Doctor, Day=OnDay{Weekday=Monday}, Time=AtTime{Hour=9, Meridiem=Am}} ."),
            Doc("the doctor comes at 9 am.", "«the doctor comes at 9 am» ."),
            Doc("the doctor comes on monday at 930 am.", "«the doctor comes on monday at 930 am» .")),

        .. Section("OptionalOf<T>",
            Doc("the mailman brings the mail.", "MailArrives{Person=MailCarrier} ."),
            Doc("the mail carrier brings the mail on friday.", "MailArrives{Person=MailCarrier, Day=OnDay{Weekday=Friday}} ."),
            Doc("the mail carrier brings the mail on payday.", "«the mail carrier brings the mail on payday» .")),

        .. Section("Optional and required repetition",
            Doc("the bird sings loudly.", "AnimalSings{Animal=Bird} ."),
            Doc("the bird sings very loudly.", "AnimalSings{Animal=Bird, Intensifiers=[Very]} ."),
            Doc("the bird sings really very loudly.", "AnimalSings{Animal=Bird, Intensifiers=[Really, Very]} ."),
            Doc("the clock goes tick.", "ClockGoes{Sounds=[Tick]} ."),
            Doc("the clock goes tick tock.", "ClockGoes{Sounds=[Tick, Tock]} ."),
            Doc("the clock goes.", "«the clock goes» ."),
            Doc("the clock goes tick, tock.", "«the clock goes tick, tock» .")),

        .. Section("OneOf<,> and OneOf<,,>",
            Doc("the dog greets us at the door.", "GreetsUs{Greeter={Dog}} ."),
            Doc("the baker greets us at the door.", "GreetsUs{Greeter={Baker}} ."),
            Doc("the horse knocks on the door.", "KnocksOnTheDoor{Visitor={TheAnimal{Animal=Horse}}} ."),
            Doc("the teacher knocks on the door.", "KnocksOnTheDoor{Visitor={ThePerson{Person=Teacher}}} ."),
            Doc("our neighbor knocks on the door.", "KnocksOnTheDoor{Visitor={OurNeighbor}} ."),
            Doc("our neighbour knocks on the door.", "«our neighbour knocks on the door» .")),

        .. Section("GlyphOneOf",
            Doc("the baker offers the dog some bread.", "OffersTreat{Person=Baker, Animal=Dog, Treat=Treat{Bread}} ."),
            Doc("the teacher offers the horse an apple.", "OffersTreat{Person=Teacher, Animal=Horse, Treat=Treat{Apple}} ."),
            Doc("the teacher offers the horse a carrot.", "«the teacher offers the horse a carrot» .")),

        .. Section("OneOf alias ([MustMatchWholeLine])",
            Doc("monday\nthe doctor comes on monday at 9 am.",
                "DayHeading{Monday}",
                "PersonComes{Person=Doctor, Day=OnDay{Weekday=Monday}, Time=AtTime{Hour=9, Meridiem=Am}} ."),
            Doc("new year's day\nthe baker opens the shop.",
                "DayHeading{NewYearsDay}",
                "BakerOpensTheShop ."),
            Doc("monday morning", "«monday morning»")),

        .. Section("ManyOf<T>",
            Doc("the cat likes fish and milk.", "AnimalLikes{Animal=Cat, Foods=[Fish, Milk | And]} ."),
            Doc("the cat likes fish, milk, and cheese.", "AnimalLikes{Animal=Cat, Foods=[Fish, Milk, Cheese | And]} ."),
            Doc("the cat likes fish, milk and cheese.", "AnimalLikes{Animal=Cat, Foods=[Fish, Milk, Cheese | And]} ."),
            Doc("the dog likes bread or cheese.", "AnimalLikes{Animal=Dog, Foods=[Bread, Cheese | Or]} ."),
            Doc("the cat likes fish.", "«the cat likes fish» ."),
            Doc("the cat likes fish milk.", "«the cat likes fish milk» .")),

        .. Section("CompoundOf<T> and [JoinedBy]",
            Doc("we have a big old dog.", "WeHaveAn{Traits=TraitList[Big, Old], Animal=Dog} ."),
            Doc("we have an old cat.", "WeHaveAn{Traits=TraitList[Old], Animal=Cat} ."),
            Doc("we have a big, old dog.", "«we have a big, old dog» ."),
            Doc("the family adopts a big, friendly dog.", "FamilyAdopts{Traits=TraitList[Big, Friendly], Animal=Dog} ."),
            Doc("the family adopts a small cat.", "FamilyAdopts{Traits=TraitList[Small], Animal=Cat} ."),
            Doc("the family adopts a big friendly dog.", "«the family adopts a big friendly dog» ."),
            Doc("flour, sugar, eggs", "ShoppingList[Flour, Sugar, Eggs]"),
            Doc("butter", "ShoppingList[Butter]"),
            Doc("flour, buttermilk", "ShoppingList[Flour, Buttermilk]"),
            Doc("flour sugar eggs", "«flour sugar eggs»"),
            Doc("flour, sugar, and eggs", "«flour, sugar, and eggs»"),
            Doc("flour, sugar. the cat eats fish.", "«flour, sugar» . AnimalEats{Animal=Cat, Food=Fish} .")),

        .. Section("Fused CompoundOf, braced one-of items, and Joiner.None",
            Doc("the teacher reads chapter x.", "ReadsChapter{Chapter=[X]} ."),
            Doc("the teacher reads chapter xiv.", "ReadsChapter{Chapter=[X, I, V]} ."),
            Doc("the teacher reads chapter xxi.", "ReadsChapter{Chapter=[X, X, I]} ."),
            Doc("the teacher reads chapter 14.", "«the teacher reads chapter 14» ."),
            Doc("the teacher reads chapter x i v.", "«the teacher reads chapter x i v» ."),
            Doc("press [ctrl][c] to copy.", "PressToAct{Keys=[KeyCap{Ctrl}, KeyCap{C}], Action=Copy} ."),
            Doc("press [ctrl][shift][z] to undo.", "PressToAct{Keys=[KeyCap{Ctrl}, KeyCap{Shift}, KeyCap{Z}], Action=Undo} ."),
            Doc("press [ctrl][2] to zoom.", "PressToAct{Keys=[KeyCap{Ctrl}, KeyCap{2}], Action=Zoom} ."),
            Doc("press [v] to paste.", "PressToAct{Keys=[KeyCap{V}], Action=Paste} ."),
            Doc("press [ctrl] [c] to copy.", "«press [ctrl] [c] to copy» ."),
            Doc("press [ctrl c] to copy.", "«press [ctrl c] to copy» ."),
            Doc("press [ctrlc] to copy.", "«press [ctrlc] to copy» ."),
            Doc("the apple costs $3.", "FruitCosts{Fruit=Apple, Price=Price{Dollars=3}} ."),
            Doc("the cherries cost $12.", "FruitCosts{Fruit=Cherry, Price=Price{Dollars=12}} ."),
            Doc("the apple costs $ 3.", "«the apple costs $ 3» .")),

        .. Section("DynamicGlyph",
            Doc("if it rains, the dog sleeps in the kitchen.", "IfWeather{Weather=Rains, Outcome=Dynamic(AnimalRests{Animal=Dog, Place=Kitchen})} ."),
            Doc("if it is sunny, the cat walks quietly to the garden.", "IfWeather{Weather=IsSunny, Outcome=Dynamic(AnimalWalks{Animal=Cat, Quietly=True, Place=Garden})} ."),
            Doc("if it snows, we sweep the floor.", "IfWeather{Weather=Snows, Outcome=Dynamic(WeSweepTheFloor)} ."),
            Doc("if it rains, the dog sleeps in the garage.", "«if it rains, the dog sleeps in the garage» .")),

        .. Section("DynamicGlyph with [TypeFilter]",
            Doc("every sunday, we water the plants.", "EveryWeekday{Weekday=Sunday, Chore=Dynamic(WeWaterThePlants)} ."),
            Doc("every monday, we sweep the floor.", "EveryWeekday{Weekday=Monday, Chore=Dynamic(WeSweepTheFloor)} ."),
            Doc("every friday, we wash the dishes.", "EveryWeekday{Weekday=Friday, Chore=Dynamic(DoTheDishes)} ."),
            Doc("every monday, the dog sleeps in the kitchen.", "«every monday, the dog sleeps in the kitchen» .")),

        .. Section("[Dependent] glyphs are not top-level",
            Doc("we water the plants.", "WeWaterThePlants ."),
            Doc("we sweep the floor.", "«we sweep the floor» .")),

        .. Section("Clauses and clause-spanning glyphs",
            Doc("the dog sleeps in the kitchen. the cat eats fish.", "AnimalRests{Animal=Dog, Place=Kitchen} . AnimalEats{Animal=Cat, Food=Fish} ."),
            Doc("the dog sleeps in the barn. the dog snores.", "AnimalRests{Animal=Dog, Place=Barn} . «the dog snores» ."),
            Doc("the dog wakes up. then it eats bread.", "MorningRoutine{Animal=Dog, Food=Bread} ."),
            Doc("the cat wakes up. then it eats fish. the bird sings loudly.", "MorningRoutine{Animal=Cat, Food=Fish} . AnimalSings{Animal=Bird} ."),
            Doc("the dog wakes up! then it eats bread.", "«the dog wakes up! then it eats bread» ."),
            Doc("the dog wakes up.", "«the dog wakes up» .")),

        .. Section("Whole-segment rule and [AllowPartialSegmentMatch]",
            Doc("good morning to everyone.", "Greeting{TimeOfDay=Morning} «to everyone» ."),
            Doc("good evening.", "Greeting{TimeOfDay=Evening} ."),
            Doc("good morning good evening.", "Greeting{TimeOfDay=Morning} Greeting{TimeOfDay=Evening} ."),
            Doc("the dog sleeps in the kitchen all day.", "«the dog sleeps in the kitchen all day» .")),

        .. Section("[TokenizationOrder]",
            Doc("the baker opens the shop.", "BakerOpensTheShop ."),
            Doc("the doctor opens the clinic.", "OpensBuilding{Person=Doctor, Building=Clinic} ."),
            Doc("the baker opens the school.", "OpensBuilding{Person=Baker, Building=School} .")),

        .. Section("{this} and multi-line documents",
            Named("Rex", "Rex barks at the baker.", "ThisBarksAt{Person=Baker} ."),
            Named("Rex", "Max barks at the baker.", "«max barks at the baker» ."),
            Doc("the dog sleeps in the kitchen.\n\nthe cat eats some milk.",
                "AnimalRests{Animal=Dog, Place=Kitchen} .",
                "AnimalEats{Animal=Cat, Food=Milk} .")),
    ];

    static TestDocument Doc(string text, params string[] expectedLines) =>
        new(TestDocument.Unnamed, text, expectedLines);

    static TestDocument Named(string name, string text, params string[] expectedLines) =>
        new(name, text, expectedLines);

    static IEnumerable<TestDocument> Section(string feature, params TestDocument[] documents) =>
        documents.Select(x => x with { Feature = feature });
}
