import copy
import json
import logging
import random
import time
from fastapi import WebSocket, WebSocketDisconnect


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
            print("Receive: ", message)
            if len(message) > 0:
                json_object = json.loads(message)
                msgtype = json_object.get("msgtype")
                if msgtype == "login":      # 상점 로그인 (실패시 소켓 끊김)
                    await self.login(json_object, websocket)
                elif msgtype == "genpin":   # PIN 번호 생성 요청
                    await self.genpin(json_object)
                elif msgtype == "order":    # 주문 처리 결과
                    await self.order(json_object)
                elif msgtype == "menu":    # 메뉴 요청 결과
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
            data = json.dumps(recvmsg)
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
            response = str(json.dumps(response))
            data = str(json.dumps(data))
            logging.getLogger().debug(f"생성된 핀 데이터 : {data}")
            self.redis.set(pin, data)

            conn = self.connections.get(shop_no)
            await conn.send_text(response)
        except Exception as e:
            logging.getLogger().error(e)

    async def order(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.order")
        try:
            key = "order_" + str(recvmsg['order_seq'])
            if self.redis.exists(key):
                self.redis.set(key, 1)
        except Exception as e:
            logging.getLogger().error(e)

    async def menu(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.menu")
        try:
            key = "order_" + str(recvmsg['order_seq'])
            # data = json.dumps(recvmsg['orderList'])
            data = json.dumps(recvmsg)
            self.redis.set(key, data)
        except Exception as e:
            logging.getLogger().error(e)

    # async def wait_order(self, order_seq):
    #     order_wait_count = 0
    #     result = False
    #     while True:
    #         if order_wait_count > 10:
    #             break
    #         key = "order_" + str(order_seq)
    #         value = self.redis.get(key)
    #         if value == "1":
    #             result = True
    #             break
    #         await asyncio.sleep(1.0)
    #         order_wait_count = order_wait_count + 1
    #     return result

    async def send_order(self, params):
        result = False
        try:
            conn = self.connections.get(str(params['shop_no']))
            if conn is None:
                return result
            response = copy.deepcopy(params)
            response['msgtype'] = "order"
            response['order_seq'] = 1
            response = str(json.dumps(response))
            key = "order_" + str(params['order_no'])
            self.redis.set(key, 0)                      # API에서 요청 받은 주문의 상태 redis에 기록
            await conn.send_text(response)              # API에서 요청 받은 주문을 agent에 전송
            result = True
        except Exception as e:
            logging.getLogger().error(f"send_order : {e}")
        return result

    async def query_menu(self, params):
        result = False
        try:
            conn = self.connections.get(str(params['shop_no']))
            if conn is None:
                return result
            response = copy.deepcopy(params)
            response['msgtype'] = "menu"
            response = str(json.dumps(response))
            await conn.send_text(response)
            result = True
        except Exception as e:
            logging.getLogger().error(f"send_order : {e}")
        return result
