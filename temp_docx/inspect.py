import sys
from docx import Document

doc = Document(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")
for i, para in enumerate(doc.paragraphs[:30]):
    runs_info = [(run.text, run.bold, run.underline) for run in para.runs]
    print(f"[{i}] TEXT: {para.text}")
    print(f"    RUNS: {runs_info}")
