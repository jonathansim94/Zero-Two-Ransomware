import os, sys

def generate_random_bin_file(filename,size_mb):
    size = 1024 * 1024 * size_mb
    with open('%s'%filename, 'wb') as fout:
        fout.write(os.urandom(size))
    pass

if __name__ == '__main__':
    generate_random_bin_file("fill.02",int(sys.argv[1]))