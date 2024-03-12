import json

def load_json(filename):
    with open(filename, 'r', encoding='utf-8') as f:
        return json.load(f)

def are_json_files_equal(file1, file2):
    data1 = load_json(file1)
    data2 = load_json(file2)
    
    return data1 == data2

# 例子
file1 = "dialog.json"
file2 = "dialog_backup.json"

if are_json_files_equal(file1, file2):
    print(f"'{file1}' 和 '{file2}' 内容一致.")
else:
    print(f"'{file1}' 和 '{file2}' 内容不一致.")

