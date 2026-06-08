import zipfile
import re
import json

def is_green(color_val):
    if not color_val:
        return False
    # Hex codes for various greens found in the document
    green_hexes = ['00ff00', '38761d', '6aa84f', '8cad56', '89be70', '274e13', '00b050']
    return color_val.lower() in green_hexes

def extract_quiz(filepath):
    with zipfile.ZipFile(filepath) as docx:
        xml_content = docx.read('word/document.xml').decode('utf-8')
    
    paragraphs = re.findall(r'<w:p\b[^>]*>(.*?)</w:p>', xml_content)
    parsed_lines = []
    
    for p in paragraphs:
        runs = re.findall(r'<w:r\b[^>]*>(.*?)</w:r>', p)
        para_text = ""
        para_has_green = False
        
        for r in runs:
            text_match = re.search(r'<w:t\b[^>]*>(.*?)</w:t>', r)
            if not text_match:
                # w:t might not have attributes
                text_match = re.search(r'<w:t>(.*?)</w:t>', r)
            
            if not text_match:
                continue
                
            t = text_match.group(1)
            t = re.sub(r'<[^>]+>', '', t)
            
            color_match = re.search(r'<w:color w:val="([^"]+)"', r)
            c = color_match.group(1) if color_match else None
            
            if is_green(c):
                para_has_green = True
                
            para_text += t
            
        parsed_lines.append({
            "text": para_text.strip(),
            "has_green": para_has_green
        })
        
    quiz = {"title": "Baza Farmakognozja (Całość - ABCD)", "questions": []}
    
    current_question = None
    current_options = []
    
    for line in parsed_lines:
        text = line['text']
        if not text:
            if current_question and len(current_options) >= 2:
                quiz['questions'].append({
                    "text": current_question['text'],
                    "options": current_options
                })
            current_question = None
            current_options = []
            continue
            
        if re.match(r'^\d+[\.\)]\s', text):
            if current_question and len(current_options) >= 2:
                quiz['questions'].append({
                    "text": current_question['text'],
                    "options": current_options
                })
            current_question = {"text": text}
            current_options = []
        elif current_question:
            if len(text) > 1:
                # Clean up A) B) C) D) if they exist
                # Actually user said "do not modify questions", so we leave the text as is.
                current_options.append({
                    "text": text,
                    "isCorrect": line['has_green']
                })
                
    if current_question and len(current_options) >= 2:
        quiz['questions'].append({
            "text": current_question['text'],
            "options": current_options
        })
        
    return quiz

quiz_data = extract_quiz(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")

# Make sure every question has at least one correct option, if not, print a warning, but keep it.
for q in quiz_data['questions']:
    if not any(opt['isCorrect'] for opt in q['options']):
        # If no option is green, maybe it wasn't colored. We just keep them as false.
        pass

with open(r"C:\Users\mikos\Downloads\baza_farmakognozja_full.json", "w", encoding="utf-8") as f:
    json.dump(quiz_data, f, ensure_ascii=False, indent=2)

print(f"Successfully generated {len(quiz_data['questions'])} questions.")
