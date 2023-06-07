import json
import tornado
from database import SessionLocal
from util import redis_pool, logger
from util.corder_exception import CorderException
from .dto import AuthDto
from .dao import AuthDao


class AuthHandler(tornado.web.RequestHandler):
    def get(self):
        success = True
        error = "0000"
        response = {"success": success, "error": error}
        self.write(response)

    def post(self):
        success = False
        error = "0000"
        # authdto = dict()
        # data = authdto.dict()
        data = dict()
        try:
            redis_data = json.loads(redis_pool.get(data['auth_key']))
            if redis_data is not None:
                if redis_data['shop_no'] == str(data['shop_no']):
                    success = True
        except Exception as e:
            logger.error(e)
        response = {"success": success, "error": error}
        self.write(response)
