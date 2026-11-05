"""Print sample IR subtrees of a given $kind from a rules.jsonl: ir_samples.py <rules.jsonl> <Kind> [count] [field=value ...]"""
import json, sys, random

path, kind = sys.argv[1], sys.argv[2]
count = int(sys.argv[3]) if len(sys.argv) > 3 else 8
filters = [a.split('=', 1) for a in sys.argv[4:]]

def walk(node):
    if isinstance(node, dict):
        yield node
        for v in node.values():
            yield from walk(v)
    elif isinstance(node, list):
        for v in node:
            yield from walk(v)

hits = []
with open(path, encoding='utf-8') as f:
    for raw in f:
        card = json.loads(raw)
        for line in card['Lines']:
            for node in walk(line['Abilities']):
                if node.get('$kind') == kind and all(str(node.get(k)) == v for k, v in filters):
                    hits.append((card['Name'], line['Text'], node))
                    break
random.seed(1)
print(f'{len(hits)} lines')
for name, text, node in random.sample(hits, min(count, len(hits))):
    print(f'--- {name}: {text}')
    print(json.dumps(node, separators=(",", ":"))[:900])
