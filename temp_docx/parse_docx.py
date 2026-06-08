import zipfile
import re
import json

def parse_docx(filepath):
    with zipfile.ZipFile(filepath) as docx:
        xml_content = docx.read('word/document.xml').decode('utf-8')
    
    paragraphs = re.findall(r'<w:p\b[^>]*>(.*?)</w:p>', xml_content)
    
    extracted = []
    for p in paragraphs:
        runs = re.findall(r'<w:r\b[^>]*>(.*?)</w:r>', p)
        
        para_text = ""
        has_bold = False
        
        for r in runs:
            is_bold = '<w:b/>' in r or '<w:b ' in r
            text_match = re.search(r'<w:t\b[^>]*>(.*?)</w:t>', r)
            if text_match:
                t = text_match.group(1)
                para_text += t
                if is_bold and t.strip():
                    has_bold = True
                    
        all_texts = re.findall(r'<w:t\b[^>]*>(.*?)</w:t>', p)
        full_text = "".join(all_texts)
        
        if full_text.strip():
            extracted.append({
                "text": full_text.strip(),
                "has_bold": has_bold
            })
            
    return extracted

parsed = parse_docx(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")
for i, item in enumerate(parsed[:30]):
    print(f"[{i}] BOLD: {item['has_bold']} | TEXT: {item['text']}")
