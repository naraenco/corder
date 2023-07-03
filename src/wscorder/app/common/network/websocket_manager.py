import copy
import json
import logging
import random
import time
from fastapi import WebSocket, WebSocketDisconnect
from sqlalchemy import text


class WebSocketManager:
    def __init__(self):
        self.connections: dict[str, WebSocket] = {}
        from common import redis_pool
        self.redis = redis_pool

    def check_connection(self, shop_no):
        if type(shop_no) == int:
            shop_no = str(shop_no)
        conn = self.connections.get(shop_no)
        if conn is None:
            return False
        return True

    async def message_handler(self, websocket: WebSocket):
        logging.getLogger().debug("ConnectionManager.message_handler")
        try:
            message = await websocket.receive_text()
            # ToDo: 메시지 디버깅
            # logging.getLogger().debug(f"Receive: {message}")
            if len(message) > 0:
                json_object = json.loads(message)
                msgtype = json_object.get("msgtype")
                if msgtype == "login":      # 상점 로그인 (실패시 소켓 끊김)
                    await self.login(json_object, websocket)
                elif msgtype == "genpin":   # PIN 번호 생성 요청
                    await self.genpin(json_object)
                elif msgtype == "order":    # 주문 처리 결과
                    await self.order(json_object)
                elif msgtype == "mylist":    # 메뉴 요청 결과
                    await self.mylist(json_object)
                elif msgtype == "tablestatus":
                    await self.tablestatus(json_object)
                elif msgtype == "tablemap":
                    await self.tablemap(json_object)
                elif msgtype == "menu":
                    await self.menu(json_object)
            elif len(message) == 0:
                print("message size = 0")
        except WebSocketDisconnect as e:
            logging.getLogger().error(e)
            # await websocket.close()
            raise
        except Exception as e:
            logging.getLogger().error(e)

    def disconnect(self, websocket):
        logging.getLogger().debug("ConnectionManager.disconnect")
        shop_no = 0
        for no, conn in self.connections:
            if conn == websocket:
                shop_no = no
                break
        del self.connections[shop_no]
        websocket.close()

    def get_socket(self, shop_no):
        try:
            if type(shop_no) == int:
                shop_no = str(shop_no)
            conn = self.connections.get(shop_no)
            return conn
        except Exception as e:
            logging.getLogger().error(f"get_socket : {e}")
        return None

    async def send(self, message: str, shop_no):
        logging.getLogger().debug("ConnectionManager.send")
        try:
            if type(shop_no) == int:
                shop_no = str(shop_no)
            conn = self.connections.get(shop_no)
            await conn.send_text(message)
        except Exception as e:
            logging.getLogger().error(e)

    async def write(self, shop_no, message):
        logging.getLogger().debug("ConnectionManager.write")
        result = None
        try:
            if type(shop_no) == int:
                shop_no = str(shop_no)
            conn = self.connections.get(shop_no)
            await conn.send_text(message)
        except Exception as e:
            logging.getLogger().error(e)
        return result

    async def broadcast(self, message: str):
        logging.getLogger().debug("ConnectionManager.broadcast")
        for shop_no, conn in self.connections:
            await conn.send_text(message)

    async def login(self, recvmsg, websocket: WebSocket):
        logging.getLogger().debug("ConnectionManager.login")
        try:
            shop_no = str(recvmsg['shop_no'])
            self.connections[shop_no] = websocket
            data = json.dumps(recvmsg, ensure_ascii=False)
            await websocket.send_text(data)
        except Exception as e:
            await websocket.close()
            logging.getLogger().error(e)

    async def genpin(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.genpin")
        try:
            shop_no = str(recvmsg['shop_no'])
            while True:
                num = random.randrange(1, 9999)
                pin = str(num).zfill(4)
                if self.redis.exists(pin) == 0:     # 중복 확인
                    break
            now = time
            data = {
                "pin": pin,
                "shop_no": shop_no,
                "created_at": now.strftime('%Y-%m-%d %H:%M:%S')
            }
            response = copy.deepcopy(data)
            response['msgtype'] = "genpin"
            response = str(json.dumps(response, ensure_ascii=False))
            data = str(json.dumps(data, ensure_ascii=False))
            logging.getLogger().debug(f"생성된 핀 데이터 : {data}")
            self.redis.set(pin, data)
            conn = self.connections.get(shop_no)
            await conn.send_text(response)
        except Exception as e:
            logging.getLogger().error(e)

    async def order(self, recvmsg):
        logging.getLogger().debug(f"ConnectionManager.order : {recvmsg}")
        try:
            key = "order_" + str(recvmsg['order_no'])
            if self.redis.exists(key):
                self.redis.set(key, 1)
        except Exception as e:
            logging.getLogger().error(e)

    async def mylist(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.mylist")
        try:
            key = "order_" + str(recvmsg['table_no'])
            data = json.dumps(recvmsg, ensure_ascii=False)
            self.redis.set(key, data)
        except Exception as e:
            logging.getLogger().error(e)

    async def tablestatus(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.tablestatus")
        # print(recvmsg)

        # try:
        #     from database import engine
        #     sql = "SELECT COUNT(*) FROM data_table_status WHERE shop_no=" + recvmsg['shop_no'] + ";"
        #     db = engine.connect()
        #     result = db.execute(text(sql)).fetchone()
        #     now = time
        #     regdate = now.strftime('%Y%m%d%H%M%S')
        #     data = json.dumps(recvmsg["data"]["TABLEUSE"], ensure_ascii=False)
        #     if result[0] == 0:
        #         sql = f"INSERT INTO data_table_map (shop_no, regdate, data) VALUES (" \
        #               f"{recvmsg['shop_no']}," \
        #               f"'{regdate}'," \
        #               f"'{data}')"
        #         db.execute(text(sql))
        #         db.commit()
        #     else:
        #         sql = f"UPDATE data_table_map " \
        #               f"SET regdate='{regdate}',data='{data}'" \
        #               f"WHERE shop_no={recvmsg['shop_no']};"
        #         db.execute(text(sql))
        #         db.commit()
        # except Exception as e:
        #     logging.getLogger().error(e)

    async def tablemap(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.tablemap")
        try:
            from database import engine
            sql = "SELECT COUNT(*) FROM data_table_map WHERE shop_no=" + recvmsg['shop_no'] + ";"
            db = engine.connect()
            result = db.execute(text(sql)).fetchone()
            now = time
            regdate = now.strftime('%Y%m%d%H%M%S')
            data = json.dumps(recvmsg["data"]["TABLEUSE"], ensure_ascii=False)
            if result[0] == 0:
                sql = f"INSERT INTO data_table_map (shop_no, regdate, data) VALUES (" \
                      f"{recvmsg['shop_no']}," \
                      f"'{regdate}'," \
                      f"'{data}')"
                db.execute(text(sql))
                db.commit()
            else:
                sql = f"UPDATE data_table_map " \
                      f"SET regdate='{regdate}',data='{data}'" \
                      f"WHERE shop_no={recvmsg['shop_no']};"
                db.execute(text(sql))
                db.commit()
        except Exception as e:
            logging.getLogger().error(e)

    async def menu(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.menu")
        try:
            from database import engine
            sql = "SELECT COUNT(*) FROM data_menu WHERE shop_no=" + recvmsg['shop_no'] + ";"
            db = engine.connect()
            result = db.execute(text(sql)).fetchone()
            now = time
            regdate = now.strftime('%Y%m%d%H%M%S')
            data = json.dumps(recvmsg["data"]["PRODUCT"], ensure_ascii=False)
            if result[0] == 0:
                sql = f"INSERT INTO data_menu (shop_no, regdate, data) VALUES (" \
                      f"{recvmsg['shop_no']}," \
                      f"'{regdate}'," \
                      f"'{data}')"
                db.execute(text(sql))
                db.commit()
            else:
                sql = f"UPDATE data_menu " \
                      f"SET regdate='{regdate}',data='{data}'" \
                      f"WHERE shop_no={recvmsg['shop_no']};"
                db.execute(text(sql))
                db.commit()
        except Exception as e:
            logging.getLogger().error(e)

    async def api_order(self, params):
        result = False
        try:
            conn = self.connections.get(str(params['shop_no']))
            if conn is None:
                logging.getLogger().error("api_order - get connection failure")
                return False
            response = copy.deepcopy(params)
            response['msgtype'] = "order"
            response['status'] = 0
            error, seq = self.redis.update_pin(str(params['otp_pin']), params['table_cd'])
            if error != "0000":
                logging.getLogger().error(f"api_order - update_pin failure code ({error})")
                return False
            response['order_seq'] = seq
            key = "order_" + str(params['order_no'])
            self.redis.set(key, 0)                      # API에서 요청 받은 주문의 상태 redis에 기록
            response = str(json.dumps(response, ensure_ascii=False))
            await conn.send_text(response)              # API에서 요청 받은 주문을 agent에 전송
            result = True
        except Exception as e:
            logging.getLogger().error(f"api_order : {e}")
        return result

    async def api_menu(self, params):
        result = False
        try:
            conn = self.connections.get(str(params['shop_no']))
            if conn is None:
                return result
            response = copy.deepcopy(params)
            response['msgtype'] = "menu"
            response = str(json.dumps(response, ensure_ascii=False))
            await conn.send_text(response)
            result = True
        except Exception as e:
            logging.getLogger().error(f"api_menu : {e}")
        return result
