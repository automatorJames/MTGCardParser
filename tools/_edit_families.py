import json, sys, collections
names = set(sys.argv[1:])
fam = collections.defaultdict(collections.Counter)
samples = collections.defaultdict(list)
def walk(node, line):
    if isinstance(node, dict):
        t = node.get("$kind")
        g = node.get("Glyph")
        if g and g in names and "Text" in node:
            fam[g][t] += 1
            if len(samples[g]) < 3: samples[g].append((line, node["Text"]))
        for v in node.values(): walk(v, line)
    elif isinstance(node, list):
        for v in node: walk(v, line)
with open(r"C:/Users/James/source/forge-study/lower-full/rules.jsonl", encoding="utf-8") as f:
    for row in f:
        card = json.loads(row)
        for line in card["Lines"]:
            if any(o.split("→")[-1].split(" as ")[0] in names or o in names for o in line["Opaque"]):
                walk(line["Abilities"], line["Text"])
for g in names:
    print("==", g, dict(fam[g]))
    for l, t in samples[g]:
        print("   ", t[:70], "||", l[:150])
