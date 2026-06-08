from docx import Document
import sys

doc = Document(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")

for i, p in enumerate(doc.paragraphs[:100]):
    for r in p.runs:
        color = None
        if r.font.color and r.font.color.rgb:
            color = str(r.font.color.rgb)
        highlight = None
        if r.font.highlight_color:
            highlight = str(r.font.highlight_color)
            
        if color or highlight:
            print(f"Para {i}: TEXT: {r.text.strip()} | COLOR: {color} | HIGHLIGHT: {highlight}")
