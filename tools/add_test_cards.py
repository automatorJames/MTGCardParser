"""Adds named cards to the engine tests' fixture (MTGEngine.Tests/Cards/cards.jsonl), keeping the cards already there: add_test_cards.py <rules.jsonl> <name> [<name> ...]"""
import json, os, subprocess, sys

source, *names = sys.argv[1:]
fixture = os.path.join(os.path.dirname(__file__), '..', 'MTGEngine.Tests', 'Cards', 'cards.jsonl')
with open(fixture, encoding='utf-8') as f:
    existing = [json.loads(line)['Name'] for line in f if line.strip()]
known = {n.lower() for n in existing}
wanted = existing + [n for n in names if n.lower() not in known]
subprocess.run([sys.executable, os.path.join(os.path.dirname(__file__), 'extract_test_cards.py'), source, fixture, *wanted], check=True)
