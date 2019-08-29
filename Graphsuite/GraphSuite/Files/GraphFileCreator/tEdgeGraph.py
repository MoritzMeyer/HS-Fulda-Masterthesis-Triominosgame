file = open('../tEdgeGraph.txt', 'w+')

for i in range(0, 6):
    for j in range(i, 6):
        for k in range(j, 6):
            file.write('%d%d %d%d %d%d\n' % (i, j, j, k, k, i))

for l in range(0, 6):
    for m in range(l, 6):
        file.write('%d%d %d%d\n' % (l, m, m , l))