"""Tally shapes of IR nodes of a $kind: ir_tally.py <rules.jsonl> <Kind> <python-expr over n> [top]"""
import json, sys, collections
path, kind, expr = sys.argv[1], sys.argv[2], sys.argv[3]
top = int(sys.argv[4]) if len(sys.argv) > 4 else 40
def walk(node):
    if isinstance(node, dict):
        yield node
        for v in node.values(): yield from walk(v)
    elif isinstance(node, list):
        for v in node: yield from walk(v)
def k(n): return n.get('$kind') if isinstance(n, dict) else None
counts, examples = collections.Counter(), {}
for raw in open(path, encoding='utf-8'):
    card = json.loads(raw)
    for line in card['Lines']:
        seen = set()
        for n in walk(line['Abilities']):
            if n.get('$kind') == kind:
                try: key = str(eval(expr, {'k': k, 'n': n}))
                except Exception as e: key = 'ERR ' + type(e).__name__
                if key in seen: continue
                seen.add(key); counts[key] += 1; examples.setdefault(key, card['Name'] + ': ' + line['Text'][:110])
for key, c in counts.most_common(top): print(f'{c:6} {key}   e.g. {examples[key]}')
