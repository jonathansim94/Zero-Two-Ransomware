import os, sys, random, string

def generate_random_txt_file(filename,size_mb):
    size = 1024 * 1024 * size_mb
    chars = ''.join([random.choice(string.ascii_letters + string.digits) for i in range(size)])  # 1

    with open(filename, 'w') as f:
        f.write(chars)

if __name__ == '__main__':
    generate_random_txt_file("fill.txt",int(sys.argv[1]))

