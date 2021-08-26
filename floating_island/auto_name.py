import os

arr = os.listdir()

for c_f in arr:
    if c_f!="auto_name.py":
        tmp_x = int(c_f[:-4])
        os.rename(c_f, "0herowad" + str(tmp_x-1) + ".png")
