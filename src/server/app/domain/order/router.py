import json
import tornado
from database import SessionLocal
from util import redis_pool, logger


class OrderHandler(tornado.web.RequestHandler):
    def get(self):
        success = True
        error = "0000"
        response = {"success": success, "error": error}
        self.write(response)

    def post(self):
        success = False
        data = tornado.escape.json_decode(self.request.body)
        print(data)

        try:
            error = redis_pool.validate_pin(data['auth_key'], str(data['shop_no']))
            # print(f"post : {error}")
            if error == "0000":
                # result = await ws_manager.send_order(data)
                result = True
                if result is True:
                    success = True
                    # o = OrderDao()
                    # db = SessionLocal()
                    # db.add(o)
                    # db.commit()
        except Exception as e:
            logger.error(f"orderpost : {e}")
            success = False
            error = "0001"

        # print("error : ", error)
        response = {"success": success, "error": error}
        self.write(response)
