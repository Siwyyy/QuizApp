import zipfile
import re
import json

def is_green(color_val, shd_val, highlight_val):
    green_hexes = ['00ff00', '38761d', '6aa84f', '8cad56', '89be70', '274e13', '00b050']
    if color_val and color_val.lower() in green_hexes:
        return True
    
    green_shades = ['d9ead3', '00ff00', '008000', '93c47d', 'b6d7a8']
    if shd_val and shd_val.lower() in green_shades:
        return True
        
    if highlight_val and highlight_val.lower() == 'green':
        return True
        
    return False

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
                text_match = re.search(r'<w:t>(.*?)</w:t>', r)
            
            if not text_match:
                continue
                
            t = text_match.group(1)
            t = re.sub(r'<[^>]+>', '', t)
            
            color_match = re.search(r'<w:color w:val="([^"]+)"', r)
            c = color_match.group(1) if color_match else None
            
            shd_match = re.search(r'<w:shd [^>]*w:fill="([^"]+)"', r)
            s = shd_match.group(1) if shd_match else None
            
            high_match = re.search(r'<w:highlight w:val="([^"]+)"', r)
            h = high_match.group(1) if high_match else None
            
            if is_green(c, s, h):
                para_has_green = True
                
            para_text += t
            
        parsed_lines.append({
            "text": para_text.strip(),
            "has_green": para_has_green
        })
        
    quiz = {"title": "Baza Farmakognozja", "questions": []}
    
    current_question = None
    current_options = []
    
    for line in parsed_lines:
        text = line['text']
        if not text:
            if current_question and len(current_options) >= 2:
                quiz['questions'].append({
                    "text": current_question['text'],
                    "options": current_options,
                    "explanation": ""
                })
            current_question = None
            current_options = []
            continue
            
        if re.match(r'^\d+[\.\)]\s', text):
            if current_question and len(current_options) >= 2:
                quiz['questions'].append({
                    "text": current_question['text'],
                    "options": current_options,
                    "explanation": ""
                })
            current_question = {"text": text}
            current_options = []
        elif current_question:
            if len(text) > 1:
                current_options.append({
                    "text": text,
                    "isCorrect": line['has_green']
                })
                
    if current_question and len(current_options) >= 2:
        quiz['questions'].append({
            "text": current_question['text'],
            "options": current_options,
            "explanation": ""
        })
        
    return quiz

quiz_data = extract_quiz(r"C:\Users\mikos\Downloads\BAZA - farmakognozja (1).docx")

valid_questions = []
for q in quiz_data['questions']:
    valid_q = {
        "text": q['text'],
        "options": q['options'],
        "explanation": ""
    }
    valid_questions.append(valid_q)

quiz_data['questions'] = valid_questions

with open(r"C:\Users\mikos\Downloads\baza_farmakognozja.json", "w", encoding="utf-8") as f:
    json.dump(quiz_data, f, ensure_ascii=False, indent=2)

print(f"Successfully generated {len(quiz_data['questions'])} questions.")
