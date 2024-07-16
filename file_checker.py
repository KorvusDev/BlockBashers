import os

def listar_archivos(directory):
    file_list = []

    for root, directory, files in os.walk(directory):
        for file in files:
            ruta_completa = os.path.join(root, file)
            file_list.append(ruta_completa)

    return file_list

current_route = os.path.dirname(os.path.abspath(__file__))

files = listar_archivos(current_route)

with open("checkfiles.txt", "w") as output_file:
    for file in files:
        output_file.write(file + "\n")

print("Archivo guardado en checkfiles.txt")
