import zipfile
import re

with zipfile.ZipFile(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx") as docx:
    xml = docx.read('word/document.xml').decode('utf-8')

shds = set(re.findall(r'<w:shd [^>]*w:fill="([^"]+)"', xml))
print("Shadings:", shds)

highs = set(re.findall(r'<w:highlight w:val="([^"]+)"', xml))
print("Highlights:", highs)

colors = set(re.findall(r'<w:color w:val="([^"]+)"', xml))
print("Colors:", colors)
