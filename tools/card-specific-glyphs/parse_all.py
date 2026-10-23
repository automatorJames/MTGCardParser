import re,sys,json,collections
sys.stdout.reconfigure(encoding="utf-8")
S=sys.argv[1]
def parse_line(body):
    """Return list of clauses; each clause is list of (kind,text) where kind in M (matched top-level glyph) or U (unmatched)."""
    i=0; clauses=[[]]; cur=clauses[-1]
    n=len(body)
    while i<n:
        c=body[i]
        if c=='⟦':
            depth=0; j=i
            while j<n:
                if body[j]=='⟦': depth+=1
                elif body[j]=='⟧':
                    depth-=1
                    if depth==0: break
                j+=1
            inner=body[i+1:j]
            name=inner.split(':',1)[0]
            cur.append(('M',name,inner)); i=j+1
        elif c=='«':
            j=body.index('»',i); cur.append(('U',body[i+1:j],None)); i=j+1
        elif body.startswith(' . ',i) or body.startswith(' .',i) and i+2>=n:
            clauses.append([]); cur=clauses[-1]; i+=3
        else:
            i+=1
    return [c for c in clauses if c]
rows=[]
for line in open(S+"/unmatched_all.txt",encoding="utf-8"):
    m=re.match(r"\s+×(\d+)\s+(.*?)\s+\[([^\]]*)\]\s*$",line)
    if not m: continue
    n=int(m.group(1)); body=m.group(2); card=m.group(3)
    cl=parse_line(body)
    rows.append(dict(n=n,card=card,clauses=cl,body=body))
json.dump(rows,open(S+"/unmatched_all.json","w",encoding="utf-8"),ensure_ascii=False)
wl=cl_whole=cl_part=0; wl_w=cw_w=cp_w=0
for r in rows:
    allU=all(all(k=='U' for k,*_ in c) for c in r['clauses'])
    for c in r['clauses']:
        us=[t for k,t,_ in c if k=='U']; uw=sum(len(t.split()) for t in us)
        if not us: continue
        if allU: wl+=1; wl_w+=uw*r['n']
        elif all(k=='U' for k,*_ in c): cl_whole+=1; cw_w+=uw*r['n']
        else: cl_part+=1; cp_w+=uw*r['n']
print("lines",len(rows),"| whole-unparsed line clauses",wl,wl_w,"| whole-unparsed clauses in partly parsed lines",cl_whole,cw_w,"| partial clauses",cl_part,cp_w)
# partial clauses: what precedes the unmatched text? top-level glyph names before it
pre=collections.Counter(); shapes=collections.Counter()
for r in rows:
    for c in r['clauses']:
        if any(k=='U' for k,*_ in c) and any(k=='M' for k,*_ in c):
            shapes[" ".join(('U' if k=='U' else k) for k,t,_ in c)]+=1
for s,cnt in shapes.most_common(40): print(cnt,s)
