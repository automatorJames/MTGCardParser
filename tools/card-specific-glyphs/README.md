# Card-specific glyph generator

Turns the grammar's remaining unparsed clauses into literal `ICardSpecific` glyphs, one per clause, named for the card:

- `<Card>Rule` - a whole unparsed clause as a top-level literal, `[TokenizationOrder(-1)]` so any general parse that
  appears later wins over it.
- `<Card>Effect` - a `[Dependent]` literal for the text after a trigger ("when/whenever ...,"), a phase trigger
  ("at the beginning of ...,"), an activated-ability cost ("...:"), or an opening "(" (reminder text), so the
  existing frame glyph keeps the structure and only the effect is opaque.

Pipeline (Python 3, run with `python -I`):

1. In the app (or via the glyphotype MCP `search_lines` tool), get every line with uncovered text:
   `search_lines scope=unmatched pattern=. limit=3000`, save the text output as `unmatched_all.txt` in a work
   folder `S`. Also save `list_glyphs` output as `glyphs.txt` (used to avoid name collisions).
2. `python -I parse_all.py S` - parses the lines into clauses (`unmatched_all.json`) and prints the gap shape.
3. `python -I gen2.py S START END` - writes `batch_START_END.cs` (C# declarations to send to `apply`), the clause
   texts (`.txt`) and metadata (`.json`). Clauses are ordered by uncovered words, longest first. A file
   `force_whole.json` (a JSON list of clause texts) forces whole-clause form for clauses whose frame split failed.
4. Send each batch's `.cs` to the MCP `apply` tool with a description. The first batch must also declare
   `public interface ICardSpecific { }` if it isn't in the grammar yet.
5. Re-run step 1-3 afterwards: any `Effect` whose frame didn't parse leaves its clause unparsed, and the second
   pass regenerates it (use `force_whole.json`) as a `Rule`.

`analyze.py S` summarizes the residuals/unparsed outputs (span recurrence, lengths).
