import zipfile
import re

def check_all_colors(filepath):
    with zipfile.ZipFile(filepath) as docx:
        xml_content = docx.read('word/document.xml').decode('utf-8')
    
    colors = set()
    highlights = set()
    
    runs = re.findall(r'<w:r\b[^>]*>(.*?)</w:r>', xml_content)
    for r in runs:
        color_match = re.search(r'<w:color w:val="([^"]+)"', r)
        highlight_match = re.search(r'<w:highlight w:val="([^"]+)"', r)
        
        if color_match: colors.add(color_match.group(1))
        if highlight_match: highlights.add(highlight_match.group(1))
            
    print("Colors:", colors)
    print("Highlights:", highlights)

check_all_colors(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")
