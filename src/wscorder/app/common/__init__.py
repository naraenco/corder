import logging
import os
import sys
from .util.jsonconfig import JsonConfig
from .util.redisutil import RedisUtil
from .util import rotation_log
from .network.websocket_manager import WebSocketManager

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


rotation_log.create_date_rotating_file_handler(
    log_system=config.get('run_system'),
    log_name="wscorder",
    log_level=logging.DEBUG,
    console=True)
logger = rotation_log.logging.getLogger()
redis_pool = RedisUtil()
ws_manager = WebSocketManager()
