# Grammar workbench: a guide for agents

You're composing a **grammar** for a corpus of short documents, one glyph at a time. A glyph is a small C# class
describing one construction of the text, e.g. "the {animal} naps on {weekday}". The grammar is *solved* when its
glyphs describe the corpus well: most words fall inside matches, and each glyph earns its keep.

You work on a shared **working definition** that the person you're collaborating with can see and edit live in the
app's Grammar Tools tab. Nothing you do touches the C# source files. Committing working changes to source
(or checkpointing or exporting a scratch grammar) is the person's decision, made in the app. Never ask for it as
part of your loop.

## Workspaces

The app holds several grammars, called **workspaces**, all scored against the same corpus. Exactly one is active,
and it's the one both you and the person are looking at. `overview` names it, and `list_workspaces` lists them all.

- **Source workspace:** the grammar compiled from the app's C# sources.
- **Scratch workspace:** a grammar kept as JSON, for experiments that shouldn't touch the real grammar.

Only create or switch workspaces when the person asks. Switching changes what they see as well.

## The loop

1. **Orient.** `overview` shows the score, coverage and biggest unmatched text. `list_glyphs` shows what exists,
   and `show` shows any definition's C# source.
2. **Find a target.** Look for recurring unmatched text:
   - `residual_phrases` lists repeated word runs;
   - `residuals` lists whole unmatched spans by cost;
   - `search_lines` finds lines by regex, and `scope=unmatched` searches only the text nothing covers.

   Prefer text that recurs with *variation* (the same frame, different words in a slot). That's where a glyph with
   a property beats literals.
3. **Draft a glyph.** Write it in C#. Check it on real lines with `tokenize` (pass your draft as `source`). If it
   doesn't match, `explain_mismatch` shows how far its pattern got and which part failed.
4. **Measure.** `evaluate` scores the working grammar with your change applied, without making it. You get the
   bit delta, the coverage delta, per-glyph contribution changes, and the lines whose tokenization changed:
   - gained: more words covered;
   - lost: fewer;
   - reshaped: same coverage, different parse.

   Read the lost and reshaped lines. A glyph that steals text from a better one, or breaks another glyph's
   matches, shows up there.
5. **Commit to the working definition.** `apply` makes the change as one step, with a short description of *why*.
   `undo` takes back the latest step; `history` lists them.
6. **Repeat**, generalizing as patterns emerge:
   - merge near-duplicate glyphs into one with a property;
   - promote repeated literal alternatives into a vocabulary;
   - extract a shared sub-phrase into a `[Dependent]` glyph used by several others.

   Removing or rewriting a glyph is as valid a step as adding one.

**Starting from scratch.** If you're asked to ignore the existing glyphs, don't remove them. Create a scratch
workspace with `create_workspace`, using `start=vocabularies` to keep the current grammar's vocabularies and nothing
else (or `start=empty` if the person wants no vocabularies either). An unused vocabulary costs nothing, and new
glyphs can use it right away, so check the vocabularies before writing a new enum. A new workspace scores exactly
the no-grammar baseline.

Work in small steps, and state what you're trying before each one. The person may be editing too. Always read the
current state rather than assuming it.

## How the score works

The score is a **minimum description length** in bits: the bits to write down the grammar, plus the bits to
write down the corpus using it. Lower is better. The baseline is the same corpus with no grammar at all.

- **Unmatched text** is expensive: every word is coded from a residual lexicon, and each distinct word is spelled
  out once. Words the grammar already spells (its literals, and the vocabulary members it uses) don't need spelling
  again, so defining a word in a vocabulary is never paid for twice. Covering recurring text is how bits are saved.
- **Every top-level glyph is also a token choice.** A match has to say which glyph it is. For a construction that
  is only a word or two long and rare in the corpus, that can cost about what leaving it unmatched does, so it may
  not pay until the corpus is bigger. That isn't a verdict on the glyph.
- **A glyph costs its own definition:** every literal character and property. A glyph that matches once rarely pays
  for itself.
- **A vocabulary costs only the members the corpus uses.** Members that never match, and synonyms that never spell
  anything, are free, because a finished grammar would cull them. So never spell out an `Alt` of words an existing
  vocabulary already covers (`Alt("creature", "land", "artifact")` instead of a `CardType` property) to "save" the
  members you don't need. Those cost nothing, and the property is the better grammar: named, captured, reusable.
- **Each match costs the choices it makes.** Those are:
  - which glyph it is;
  - which *frame* it took (its text with captures masked out; every `Alt`/`Opt`/`Pattern` variation creates
    frames);
  - what each property captured.

  Choices that are always the same cost nothing, and a very variable frame costs more.
- **An open-ended regex pays to spell what it swallows.** Text matched by a `Pattern` with `+`, `*` or a character
  class (`[^.]+`, `\w+`, `.`) is masked out of the frame and spelled out the first time each distinct text appears.
  A catch-all costs about what writing its texts out would, so it never beats modeling them. Use patterns for
  small closed variations (`an?`, `cards?`), not to hide text.
- A glyph's **net bits** = what its matches would cost as unmatched text − what they cost as matches − its own
  definition. Positive means it pays for itself. A nested-only glyph's net is minus its definition cost; the glyphs
  using it are credited instead.

So the score rewards the grammar a person would call *right*: general templates with slots for what varies,
vocabularies for closed word sets, and nothing that matches too little to justify itself. **Coverage** (the share
of words inside matches) is the other headline number. The score is the judge when the two disagree.

## Notation in tool output

- `⟦GlyphName: matched text⟧` is a top-level match.
- `«text»` is unmatched text.
- Nested renderings also show captures:
  - `⟦Property: …⟧` is a nested glyph;
  - `⟦Property→Glyph: …⟧` is what a `DynamicGlyph` property resolved to;
  - `⟦Property: «…»⟧` is text a `[AllowUnmatched]` dynamic holds unresolved;
  - `⟦Property=text⟧` is a terminal (vocabulary member, bool, or number).
- A *frame* such as `the {Animal} naps on {Day}` is a match's text with its captures masked out.

Corpus text is lower-cased before tokenizing, and a document's own name is replaced with `{this}`. Every `{this}` in
a match is a singular referent, so a later "it" resolves to the document. Write it as a literal where it's the only
thing that fits. Where the document is one of several things that can fill a property, use the built-in `This` glyph,
e.g. `OneOf<This, …>`.

## Writing glyphs

Send one or more declarations as C# source, without a namespace or usings. They are read as definitions, not
compiled, so they may refer to anything in the working grammar. Declaring a name that already exists replaces
it. Remove definitions by name with `remove`.

**Documentation.** A glyph's `///` doc comment is part of its definition: it's shown with the glyph, and committed
with it. Three tags in it are read as the glyph's documentation, each optional and each free text (anything else
in the comment is kept, but isn't part of it):

```csharp
/// <summary>What the glyph is for, and why it exists. Doc-comment markup like <see cref="OnDay"/> is fine here.</summary>
/// <exampledoc>The name of one corpus document the glyph is meant for</exampledoc>
/// <examplecapture>the dog naps on monday</examplecapture>
public class AnimalNaps : Glyph
```

`<examplecapture>` is just the part of that document's text the glyph captures. When you replace a glyph, send its
documentation along (updated, if it's out of date), or the replacement drops it. Documentation isn't grammar: it
changes no score, and a step that only documents glyphs needn't take bits off.

```csharp
public class AnimalNaps : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), Alt("naps", "sleeps"), Opt("soundly"), Prop(Day)];

    public Animal Animal { get; set; }   // a vocabulary (enum)
    public OnDay Day { get; set; }       // another glyph, nested
}
```

**Nibs** are the glyph's parts, in order. By default they're joined by a single space.

| Nib | Matches |
|---|---|
| `"text"` | the text exactly, as literal characters (no regex) |
| `Alt("a", "b")` | exactly one of the texts |
| `Opt(nib)` | the nib, or nothing |
| `Plural(nib)` | the nib, singular or plural: `Plural("card")` matches card and cards, `Plural(Prop(CardType))` creature and creatures |
| `Pattern(@"regex")` | a regex, for what the others can't express, e.g. `Pattern("an?")` |
| `Prop(Name)` | the property `Name` |

With no `Nibs` override, a glyph matches its properties in declaration order. With no properties either, it
matches `[RegexPattern("…")]` if given, else its own name, friendly-cased (`WeSweepTheFloor` → "we sweep the
floor"). `public override Joiner Joiner => Joiner.None;` joins nibs with nothing instead of a space.

**Property types:**
- **A glyph**, nested.
- **A vocabulary** (an enum): a closed set of words. By default each member matches its friendly-cased name; give
  synonyms with `[RegexPattern("dog", "hound")]` on the member. `[OptionalPlural]` on the enum accepts plurals of
  every member. Declare a new enum in the same source as the glyph that first uses it.
- **`bool`**, a presence flag: `[RegexPattern("soundly")] public bool Soundly { get; set; }` is true when the text
  is there.
- **`int`**: digits, or a custom `[RegexPattern]`.
- **`DynamicGlyph`**: whatever glyph matches the captured text, resolved at match time. `[TypeFilter(typeof(IMarker))]`
  restricts it to glyphs implementing an empty marker interface (`public interface IMarker { }`, listed after the
  base class: `class X : Glyph, IMarker`). Use it for "if …, {any effect}" constructions. By default, when no glyph
  matches the captured text, the whole match fails.

  `[AllowUnmatched]` on the property changes that: the text is kept as unmatched text inside the match (shown as
  `⟦Effect: «…»⟧`) instead of failing it. It's still charged and counted as unmatched text, but the surrounding
  frame is paid for immediately, and the held text resolves by itself once some glyph matches it. This is how to
  model an outer construction (a trigger, a condition, a cost) before its inner parts. `residuals` and
  `search_lines scope=unmatched` include held text, so they show which inner parts to model next.
- **`OneOf<A, B>` / `OneOf<A, B, C>`**: exactly one of two or three types. Value types must be nullable:
  `OneOf<Animal?, Person?>`.
- **`ManyOf<T>`**: a list with a conjunction ("x, y, and z", "x or y").
- **`CompoundOf<T>`**: a joined run without a conjunction. It's comma-joined by default;
  `[JoinedBy(Joiner.Space)]` changes that.
- **`OptionalOf<T>`**: an optional glyph.

`[Optional]` makes a property optional; an optional value type must be nullable (`Animal?`).

**Other kinds of glyph:**
- `class Treat : GlyphOneOf` has properties that are named alternatives, exactly one of which matches.
- `class TraitList : CompoundOf<Trait>;` is a named alias of a primitive, so it can carry class attributes.

**Class attributes:**

| Attribute | Effect |
|---|---|
| `[Dependent]` | only matched inside another glyph, never on its own |
| `[AllowPartialClauseMatch]` | see the rule below |
| `[TokenizationOrder(n)]` | changes which glyph is tried first: n ≥ 0 goes before glyphs without one (lowest first), n < 0 goes after all of them |

Without an order, glyphs with longer patterns are tried first, and the first glyph to match a clause wins it.

**The whole-clause rule.** A top-level glyph must match an entire clause: everything between line starts and
periods. Otherwise it doesn't match at all, so "the dog naps" won't match inside "the dog naps on monday".
`[AllowPartialClauseMatch]` relaxes this for one glyph. Use it sparingly, since partial matches hide unmodeled
text. For shared fragments, prefer `[Dependent]` glyphs nested inside whole-clause ones.

**Periods.** Every period ends a clause, and the tokenizer emits it (along with any `)` or `"` straight after it) as
a clause break, shown as ` . `. To cover several sentences, put the period inside a literal nib (`"by it. they
can't be regenerated"`) or write it as its own `"."` nib. A clause's closing period is always a break, so one written
at the end of a top-level glyph is simply dropped. It's an error on a `[Dependent]` or `[AllowPartialClauseMatch]`
glyph, or on one another glyph uses as a property, because there the period would fall inside a larger match. No
pattern, `Alt` or `Opt` may contain a period.

**Back-references** ("it", "they", "that creature") are glyphs deriving from `BackReference`. They match like any glyph.
After the line is tokenized, each one resolves to the most recent earlier **referent** in its line that it agrees with.
If nothing agrees, it stays unresolved and is counted.
- `[Referent]` on a property makes its captured value a referent. On a glyph class, every match of the class is one.
  Mark only what something later refers back to.
- A referent's kind is its type, never written: an enum property's kind is the enum (`CardType`), a glyph's is the
  glyph, and a one-of's is whichever alternative matched. `{this}` is always a referent, of kind `This`.
- `BackReference<T>` refers only to referents of kind `T`: `class ThatCard : BackReference<CardType>` matching
  "that creature" skips a more recent `{this}` or player. Plain `BackReference` refers to any kind.
- The standard pronouns are built in, so use them as property types without declaring them: `It`, `Its`, `Itself`
  (singular) and `They`, `Them`, `Their`, `Themselves`, `These`, `Those` (plural). Don't declare a glyph with one of
  these names. Where a pronoun can only mean one kind of thing in its place ("they" in "creatures they control" is a
  player), declare a `BackReference<T>` of that kind matching it, and use it in that place.
- Number is optional on both sides: `[Referent(GrammaticalNumber.Plural)]` says a referent is several things, and
  `[Singular]` or `[Plural]` on a back-reference says what it refers to, so "it" skips plural referents and "they"
  singular ones. Leave it off unless that's needed.
- Every back-reference must be `[Dependent]`.
- `[RefersTo(nameof(Target))]` on a back-reference property binds it to a sibling property directly, skipping the search.

## Habits that work

- **Look at real lines before writing.** Use `search_lines` for the phrase and look at the variation around it.
  Write the glyph for the family of lines, not for one line.
- **Check the reshaped and lost lines after every evaluate.** A score win that breaks another glyph's matches
  usually means a pattern is too broad.
- **Prefer properties to `Alt`** when the choice means something: a vocabulary's members are named, reusable, and
  cheaper once shared.
- **Reuse before adding.** Check `list_glyphs` and `show` for an existing glyph or vocabulary that already says
  what you need.
- **Name things for what they mean in the corpus's domain.** Names are the part only you can supply, and the
  person reads them.
- **When a change is worse, say so and undo it** rather than piling fixes on top. When the score stalls, try
  refactoring (merge, generalize, extract) before adding more glyphs.
