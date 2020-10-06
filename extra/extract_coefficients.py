import csv

Di = [None] * 512

with open("coefficients.txt", "r") as f:
    reader = csv.reader(f, delimiter=',')
    for row in reader:
        index = int(row[0])
        value = float(row[1])
        Di[index] = value

print("{ ", end="")

for i, v in enumerate(Di):
    if v > 0:
        print(" ", end="")

    if (i == 511):
        print(f"{v:.9f} ", end="")
    else:
        print(f"{v:.9f}, ", end="")

print("}")