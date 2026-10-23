import re,sys,json,unicodedata,collections
sys.stdout.reconfigure(encoding="utf-8")
S=sys.argv[1]
existing=set()
for line in open(S+"/glyphs.txt",encoding="utf-8"):
    m=re.match(r"\s*[-\d,]+\s+[\d,]+\s+[\d,]+\s+[\d,]+\s+(\w+)",line)
    if m: existing.add(m.group(1))
# also names from source dir
import os
for f in os.listdir(r"C:\Users\James\source\repos\automatorJames\MTGCardParser\MTGGlyphs\GlyphDefinitions"):
    if f.endswith(".cs"): existing.add(f[:-3])

def ident(card):
    card=card.split(" // ")[0]
    s=unicodedata.normalize("NFKD",card).replace("'","")
    s="".join(c for c in s if not unicodedata.combining(c))
    s=re.sub(r"[^A-Za-z0-9 ]+"," ",s)
    s="".join(w[:1].upper()+w[1:] for w in s.split())
    if not s or s[0].isdigit(): s="Card"+s
    return s

def cs_str(t):
    return '"'+t.replace("\\","\\\\").replace('"','\\"')+'"'

def nibs_for(segs):
    """segs: list of clause texts; returns C# nib list string"""
    nibs=[]
    for i,seg in enumerate(segs):
        if i>0: nibs.append('"."')
        parts=seg.split("{this}")
        for j,p in enumerate(parts):
            if j>0: nibs.append("Nib.This")
            p=p.strip()
            if p: nibs.append(cs_str(p))
    return "["+", ".join(nibs)+"]"

def doc(card,text,kind):
    what={"Rule":"rules text","Effect":"effect"}[kind]
    summary=f"Card-specific {what} of {card}, held as one literal for the game engine until the grammar models it."
    return f"/// <summary>{summary}</summary>\n/// <exampledoc>{card}</exampledoc>"

def whole(name,card,segs):
    text=" . ".join(segs)
    return f"{doc(card,text,'Rule')}\n[TokenizationOrder(-1)]\npublic class {name} : Glyph, ICardSpecific\n{{\n    public override Nib[] Nibs => {nibs_for(segs)};\n}}\n"

def effect(name,card,text):
    return f"{doc(card,text,'Effect')}\n[Dependent]\npublic class {name} : Glyph, ICardSpecific\n{{\n    public override Nib[] Nibs => {nibs_for([text])};\n}}\n"

SPLITS=[
 (r"^((?:when|whenever) [^,]+), (.+)$","trigger"),
 (r"^(at the beginning of [^,]+), (.+)$","phase"),
 (r"^([^:]+): (.+)$","activated"),
]
def split_frame(seg):
    for rx,kind in SPLITS:
        m=re.match(rx,seg)
        if m and len(m.group(2).split())>=3: return kind,m.group(1),m.group(2)
    return None

def load_unparsed():
    lines=[]
    for line in open(S+"/unparsed.txt",encoding="utf-8"):
        m=re.match(r"\s+×(\d+)\s+(.*)\s+\[(.*)\]\s*$",line)
        if not m: continue
        n=int(m.group(1)); body=m.group(2); card=m.group(3)
        segs=re.findall(r"«([^»]*)»",body)
        words=sum(len(s.split()) for s in segs)
        lines.append(dict(n=n,words=words,card=card,segs=segs,text=" . ".join(segs)))
    return lines

if __name__=="__main__":
    lines=load_unparsed()
    used=collections.Counter()
    out=[]
    for L in lines:
        base=ident(L["card"]); used[base]+=1
        L["base"]=base+("" if used[base]==1 else str(used[base]))
    json.dump(lines,open(S+"/unparsed.json","w",encoding="utf-8"),ensure_ascii=False,indent=0)
    print(len(lines),"lines; name collisions with existing glyphs:",[L["base"] for L in lines if L["base"]+"Rule" in existing or L["base"]+"Effect" in existing][:20])
    print("frame-splittable:",sum(1 for L in lines if len(L["segs"])==1 and split_frame(L["segs"][0])))
