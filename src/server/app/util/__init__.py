import os
import sys
from .jsonconfig import JsonConfig

uppath = lambda _path, n: os.sep.join(_path.split(os.sep)[:-n])

_curpath = os.path.dirname(os.path.abspath(__file__))
_curpath = uppath(_curpath, 1)
sys.path.append(_curpath)
pathfile = _curpath + "/config.json"

config = JsonConfig()
try:
    config.load(pathfile)
except IOError as e:
    print(e)
