# -*- coding: utf-8 -*-
import os
import re
from fpdf import FPDF

class PDF(FPDF):
    def header(self):
        if self.page_no() > 1:
            self.set_font('Helvetica', 'B', 8)
            self.set_text_color(100, 116, 139)
            self.cell(0, 5, 'UPDS | PROGRAMACION WEB II - PROYECTO FINAL ECOWASH MOVIL', 0, new_x='LMARGIN', new_y='NEXT', align='R')
            self.set_draw_color(203, 213, 225)
            self.line(10, 14, 200, 14)
            self.ln(6)

    def footer(self):
        if self.page_no() > 1:
            self.set_y(-15)
            self.set_font('Helvetica', 'I', 8)
            self.set_text_color(148, 163, 184)
            self.cell(0, 10, f'Pagina {self.page_no()}/{{nb}}', 0, align='C')

pdf = PDF()
pdf.alias_nb_pages()
pdf.set_auto_page_break(auto=True, margin=15)

# Cover Page
pdf.add_page()
pdf.ln(10)
pdf.set_font('Helvetica', 'B', 14)
pdf.set_text_color(30, 58, 138)
pdf.cell(0, 8, 'UNIVERSIDAD PRIVADA DOMINGO SAVIO', align='C', new_x='LMARGIN', new_y='NEXT')
pdf.set_font('Helvetica', 'B', 11)
pdf.set_text_color(71, 85, 105)
pdf.cell(0, 6, 'FACULTAD DE INGENIERIA - INGENIERIA DE SISTEMAS', align='C', new_x='LMARGIN', new_y='NEXT')
pdf.ln(20)

pdf.set_font('Helvetica', 'B', 20)
pdf.set_text_color(30, 64, 175)
pdf.multi_cell(0, 10, 'DOCUMENTACION DEL PROYECTO FINAL\nEcoWash Movil', align='C')
pdf.ln(5)

pdf.set_font('Helvetica', '', 11)
pdf.set_text_color(100, 116, 139)
pdf.multi_cell(0, 6, 'Plataforma de Gestion Comercial y Servicio de Lavado de Vehiculos a Domicilio\nAsignatura: Programacion Web II', align='C')
pdf.ln(20)

pdf.set_fill_color(248, 250, 252)
pdf.set_draw_color(203, 213, 225)
pdf.rect(20, 120, 170, 95, style='DF')

pdf.set_xy(25, 125)
pdf.set_font('Helvetica', 'B', 10)
pdf.set_text_color(30, 58, 138)
pdf.cell(45, 7, 'Integrantes:', new_x='RIGHT')
pdf.set_font('Helvetica', '', 10)
pdf.set_text_color(15, 23, 42)
pdf.cell(100, 7, '1. Gabriel Alcon', new_x='LMARGIN', new_y='NEXT')

pdf.set_x(70)
pdf.cell(100, 7, '2. Olver Alvarez', new_x='LMARGIN', new_y='NEXT')
pdf.set_x(70)
pdf.cell(100, 7, '3. Kevin Nunez', new_x='LMARGIN', new_y='NEXT')
pdf.set_x(70)
pdf.cell(100, 7, '4. Christian Guizada', new_x='LMARGIN', new_y='NEXT')

pdf.ln(4)
pdf.set_x(25)
pdf.set_font('Helvetica', 'B', 10)
pdf.set_text_color(30, 58, 138)
pdf.cell(45, 7, 'Tecnologias:', new_x='RIGHT')
pdf.set_font('Helvetica', '', 10)
pdf.set_text_color(15, 23, 42)
pdf.cell(100, 7, 'Vue.js 3 + ASP.NET Core Web API + MySQL + Docker', new_x='LMARGIN', new_y='NEXT')

pdf.set_x(25)
pdf.set_font('Helvetica', 'B', 10)
pdf.set_text_color(30, 58, 138)
pdf.cell(45, 7, 'Turno / Gestion:', new_x='RIGHT')
pdf.set_font('Helvetica', '', 10)
pdf.set_text_color(15, 23, 42)
pdf.cell(100, 7, 'Manana | Gestion I/2026', new_x='LMARGIN', new_y='NEXT')

pdf.set_x(25)
pdf.set_font('Helvetica', 'B', 10)
pdf.set_text_color(30, 58, 138)
pdf.cell(45, 7, 'Fecha:', new_x='RIGHT')
pdf.set_font('Helvetica', '', 10)
pdf.set_text_color(15, 23, 42)
pdf.cell(100, 7, '30 de Julio de 2026', new_x='LMARGIN', new_y='NEXT')

pdf.set_x(25)
pdf.set_font('Helvetica', 'B', 10)
pdf.set_text_color(30, 58, 138)
pdf.cell(45, 7, 'Ubicacion:', new_x='RIGHT')
pdf.set_font('Helvetica', '', 10)
pdf.set_text_color(15, 23, 42)
pdf.cell(100, 7, 'Santa Cruz de la Sierra - Bolivia', new_x='LMARGIN', new_y='NEXT')

# Read PROYECTO_FINAL.md
with open('PROYECTO_FINAL.md', 'r', encoding='utf-8') as f:
    text = f.read()

def clean(t):
    return t.replace('**', '').replace('🚗', '').replace('👥', '').replace('📌', '').replace('📋', '').replace('🏛️', '').replace('🧠', '').replace('🔐', '').replace('👤', '').replace('🔄', '').replace('🐳', '').replace('🗄️', '').replace('⚡', '').replace('📋', '').replace('🔒', '').replace('📊', '').replace('🧪', '').replace('🚀', '').replace('🛑', '').replace('🏗️', '').replace('📁', '').replace('✅', '').replace('❌', '').replace('💡', '').replace('🌐', '').replace('⚙️', '').replace('📖', '').replace('💬', '').replace('ñ', 'n').replace('Ñ', 'N').replace('á', 'a').replace('é', 'e').replace('í', 'i').replace('ó', 'o').replace('ú', 'u').replace('Á', 'A').replace('É', 'E').replace('Í', 'I').replace('Ó', 'O').replace('Ú', 'U')

lines = text.split('\n')
pdf.add_page()

in_code = False
code_lines = []
in_table = False
table_rows = []

for line in lines:
    line_clean = clean(line.strip())
    
    if line_clean.startswith('`'):
        if in_code:
            in_code = False
            pdf.set_font('Courier', '', 8)
            pdf.set_fill_color(241, 245, 249)
            pdf.set_text_color(30, 41, 59)
            code_text = '\n'.join(code_lines)
            pdf.set_x(10)
            pdf.multi_cell(190, 4, code_text, fill=True)
            pdf.ln(2)
            code_lines = []
        else:
            in_code = True
            code_lines = []
        continue
        
    if in_code:
        code_lines.append(line_clean)
        continue

    if line_clean.startswith('|') and line_clean.endswith('|'):
        if '---|---' in line_clean or '|---|' in line_clean:
            continue
        cells = [c.strip() for c in line_clean.strip('|').split('|')]
        table_rows.append(cells)
        in_table = True
        continue
    else:
        if in_table and table_rows:
            pdf.set_font('Helvetica', '', 8)
            pdf.set_text_color(15, 23, 42)
            pdf.set_x(10)
            with pdf.table() as t:
                for r in table_rows:
                    row_obj = t.row()
                    for cell in r:
                        row_obj.cell(cell if cell else '-')
            pdf.ln(3)
            table_rows = []
            in_table = False

    if not line_clean:
        pdf.ln(2)
        continue

    pdf.set_x(10)
    if line_clean.startswith('# '):
        pdf.set_font('Helvetica', 'B', 15)
        pdf.set_text_color(30, 58, 138)
        pdf.multi_cell(190, 8, line_clean[2:])
        pdf.ln(2)
    elif line_clean.startswith('## '):
        pdf.set_font('Helvetica', 'B', 12)
        pdf.set_text_color(37, 99, 235)
        pdf.multi_cell(190, 7, line_clean[3:])
        pdf.ln(2)
    elif line_clean.startswith('### '):
        pdf.set_font('Helvetica', 'B', 10)
        pdf.set_text_color(30, 64, 175)
        pdf.multi_cell(190, 6, line_clean[4:])
        pdf.ln(1)
    elif line_clean.startswith('- ') or line_clean.startswith('* '):
        pdf.set_font('Helvetica', '', 9.5)
        pdf.set_text_color(30, 41, 59)
        pdf.multi_cell(190, 5, '  * ' + line_clean[2:])
    elif line_clean.startswith('---'):
        pdf.set_draw_color(226, 232, 240)
        pdf.line(10, pdf.get_y(), 200, pdf.get_y())
        pdf.ln(3)
    else:
        pdf.set_font('Helvetica', '', 9.5)
        pdf.set_text_color(30, 41, 59)
        pdf.multi_cell(190, 5, line_clean)

if in_table and table_rows:
    pdf.set_font('Helvetica', '', 8)
    pdf.set_x(10)
    with pdf.table() as t:
        for r in table_rows:
            row_obj = t.row()
            for cell in r:
                row_obj.cell(cell if cell else '-')

pdf.output('PROYECTO_FINAL.pdf')
print('SUCCESS: PROYECTO_FINAL.pdf generated!')
