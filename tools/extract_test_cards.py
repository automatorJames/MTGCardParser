"""Copies named cards' lowered rules (one CardRules per line) out of a rules.jsonl into a fixture file for the engine tests.

usage: python extract_test_cards.py <rules.jsonl> <out.jsonl> <name> [<name> ...]
Picks the earliest printing (first occurrence) of each name.
"""
import json, sys

source, out, *names = sys.argv[1:]
wanted = {n.lower(): n for n in names}
found = {}
with open(source, encoding="utf-8") as f:
    for row in f:
        card = json.loads(row)
        key = card["Name"].lower()
        if key in wanted and key not in found:
            found[key] = row.rstrip("\n")
missing = [wanted[k] for k in wanted if k not in found]
with open(out, "w", encoding="utf-8") as f:
    for k in wanted:
        if k in found:
            f.write(found[k] + "\n")
print(f"wrote {len(found)} cards; missing: {missing}")
