import os

ls=os.listdir()
num_lines=0

for it in ls:
    if (it[len(it)-1]=='s')and(it[len(it)-2]=='c'):
        f=open(it, 'r')

        tx=list(f.read())

        for i in range(0, len(tx)):
            if (tx[i]=='\\')and((tx[i+1]!='n')and(tx[i+1]!='r')):
                tx[i]='/'

        #print("".join(tx))
        f.close()

        f=open(it, 'w')
        f.write("".join(tx))
        f.close()

print(num_lines)
