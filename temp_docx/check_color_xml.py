import zipfile
import re

def check_colors(filepath):
    with zipfile.ZipFile(filepath) as docx:
        xml_content = docx.read('word/document.xml').decode('utf-8')
    
    paragraphs = re.findall(r'<w:p\b[^>]*>(.*?)</w:p>', xml_content)
    
    for i, p in enumerate(paragraphs[:50]):
        runs = re.findall(r'<w:r\b[^>]*>(.*?)</w:r>', p)
        for r in runs:
            text_match = re.search(r'<w:t\b[^>]*>(.*?)</w:t>', r)
            if not text_match:
                continue
            text = text_match.group(1).strip()
            if not text:
                continue
            
            color_match = re.search(r'<w:color w:val="([^"]+)"', r)
            highlight_match = re.search(r'<w:highlight w:val="([^"]+)"', r)
            
            if color_match or highlight_match:
                c = color_match.group(1) if color_match else "None"
                h = highlight_match.group(1) if highlight_match else "None"
                print(f"[{i}] TEXT: {text} | COLOR: {c} | HIGHLIGHT: {h}")

check_colors(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")
