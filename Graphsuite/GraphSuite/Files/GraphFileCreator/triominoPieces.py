file = open('../triominoPieces.txt', 'w+')


for i in range(0, 6):
    for j in range(i, 6):
        for k in range(j, 6):
            file.write("%d %d %d\n" % (i, j, k))