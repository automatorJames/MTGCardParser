import re,sys,json,collections,os
sys.stdout.reconfigure(encoding="utf-8")
S=sys.argv[1]; sys.path.insert(0,S)
from gen import ident,cs_str,nibs_for,doc,existing
rows=json.load(open(S+"/unmatched_all.json",encoding="utf-8"))
# collect unparsed clauses
clauses=[]
for r in rows:
    for ci,c in enumerate(r['clauses']):
        if all(k=='U' for k,*_ in c):
            text=" ".join(t for k,t,_ in c)
            clauses.append(dict(card=r['card'],n=r['n'],text=text,words=len(text.split())))
# dedupe identical texts (same text may appear in several cards, e.g. reprints share names anyway)
seen={}
for c in clauses:
    if c['text'] in seen: seen[c['text']]['n']+=c['n']; continue
    seen[c['text']]=c
clauses=list(seen.values())
clauses.sort(key=lambda c:(-c['words']*c['n'],c['card']))
# names
used=collections.Counter()
for c in clauses:
    b=ident(c['card'])
    if b+"Rule" in existing or b+"Effect" in existing: b+="Card"  # a card named like a keyword (Flashback, Rebound) must not replace that keyword's glyph
    used[b]+=1; c['base']=b; c['index']=used[b]
def name_for(c,kind):
    # <Card>Rule, <Card>Rule2, ... in the order the clauses are generated; numbering continues past glyphs the grammar already has
    i=c['index']
    while True:
        name=c['base']+kind+("" if i==1 else str(i))
        if name not in existing: return name
        i+=1
PREFIX=r"^(?:(?P<aw>[a-z][a-z' -]{1,30}?) — )?"
SPLITS=[
 (PREFIX+r"(?P<frame>(?:when|whenever) [^,]+), (?P<rest>.+)$","trigger"),
 (PREFIX+r"(?P<frame>at the beginning of [^,]+), (?P<rest>.+)$","phase"),
 (PREFIX+r"(?P<frame>at end of combat), (?P<rest>.+)$","phase"),
 (PREFIX+r"(?P<frame>[^:\"]{1,80}): (?P<rest>.+)$","activated"),
 (r"^\((?P<rest>.+)$","reminder"),
]
def split_frame(text):
    for rx,kind in SPLITS:
        m=re.match(rx,text)
        if m and len(m.group('rest').split())>=2: return kind,m.group('rest')
    return None
def whole(c):
    name=name_for(c,"Rule")
    return name,f"{doc(c['card'],c['text'],'Rule')}\n[TokenizationOrder(-1)]\npublic class {name} : Glyph, ICardSpecific\n{{\n    public override Nib[] Nibs => {nibs_for([c['text']])};\n}}\n"
def effect(c,rest):
    name=name_for(c,"Effect")
    return name,f"{doc(c['card'],rest,'Effect')}\n[Dependent]\npublic class {name} : Glyph, ICardSpecific\n{{\n    public override Nib[] Nibs => {nibs_for([rest])};\n}}\n"
json.dump(clauses,open(S+"/clauses.json","w",encoding="utf-8"),ensure_ascii=False)
print("clauses",len(clauses),"words",sum(c['words']*c['n'] for c in clauses),"splittable",sum(1 for c in clauses if split_frame(c['text'])))
def build(batch,force_whole=set()):
    src=[]; texts=[]
    for c in batch:
        sp=split_frame(c['text'])
        if sp and c['text'] not in force_whole:
            name,s=effect(c,sp[1]); c['shape']='effect'
        else:
            name,s=whole(c); c['shape']='whole'
        c['glyph']=name; src.append(s); texts.append(c['text'])
    return "\n".join(src),"\n".join(texts)
if __name__=="__main__" and len(sys.argv)>3:
    start,end=int(sys.argv[2]),int(sys.argv[3])
    force=set(json.load(open(S+"/force_whole.json",encoding="utf-8"))) if os.path.exists(S+"/force_whole.json") else set()
    batch=clauses[start:end]
    src,texts=build(batch,force)
    open(S+f"/batch_{start}_{end}.cs","w",encoding="utf-8").write(src)
    open(S+f"/batch_{start}_{end}.txt","w",encoding="utf-8").write(texts)
    json.dump(batch,open(S+f"/batch_{start}_{end}.json","w",encoding="utf-8"),ensure_ascii=False)
    print("batch",start,end,"glyphs",len(batch),"effects",sum(1 for c in batch if c['shape']=='effect'),"chars",len(src))
