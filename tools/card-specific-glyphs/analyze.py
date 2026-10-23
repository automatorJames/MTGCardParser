import re,sys
sys.stdout.reconfigure(encoding="utf-8")
S=sys.argv[1]
# residuals
tot=0; by_count={}; big=0; bigw=0; small=0; smallw=0; spans=[]
for line in open(S+"/residuals.txt",encoding="utf-8"):
    m=re.match(r"\s+(\d[\d,]*)\s+(\d+)\s+(\d+)\s+(.*)$",line)
    if not m: continue
    bits=int(m.group(1).replace(",",""));cnt=int(m.group(2));w=int(m.group(3));txt=m.group(4)
    spans.append((bits,cnt,w,txt))
print("spans",len(spans))
tw=sum(c*w for b,c,w,t in spans); print("total uncovered words (count*words)",tw)
for lo,hi in [(1,1),(2,2),(3,5),(6,20),(21,10**6)]:
    sel=[s for s in spans if lo<=s[1]<=hi]
    print(f"count {lo}-{hi}: spans {len(sel)}, words {sum(c*w for b,c,w,t in sel)}, bits {sum(b for b,c,w,t in sel)}")
for lo,hi in [(1,3),(4,8),(9,15),(16,25),(26,10**6)]:
    sel=[s for s in spans if lo<=s[2]<=hi]
    print(f"span length {lo}-{hi} words: spans {len(sel)}, words {sum(c*w for b,c,w,t in sel)}")
# unparsed lines
lines=[];
for line in open(S+"/unparsed.txt",encoding="utf-8"):
    m=re.match(r"\s+×(\d+)\s+(.*)\s+\[(.*)\]\s*$",line)
    if not m: continue
    n=int(m.group(1)); body=m.group(2); card=m.group(3)
    segs=re.findall(r"«([^»]*)»",body)
    words=sum(len(s.split()) for s in segs)
    lines.append((n,words,card,segs,body))
print("unparsed lines",len(lines),"occurrences",sum(n for n,*_ in lines),"words",sum(n*w for n,w,*_ in lines))
multi=[l for l in lines if len(l[3])>1]; print("multi-clause unparsed lines",len(multi))
import collections
print("by length:",collections.Counter(min(l[1]//10*10,60) for l in lines))
