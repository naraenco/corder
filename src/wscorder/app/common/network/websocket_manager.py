import copy
import json
import logging
import random
import time
from fastapi import WebSocket, WebSocketDisconnect


class WebSocketManager:
    def __init__(self):
        self.connections: dict[str, WebSocket] = {}

    async def message_handler(self, websocket: WebSocket):
        logging.getLogger().debug("ConnectionManager.message_handler")
        try:
            message = await websocket.receive_text()
            print("Receive: ", message)
            if len(message) > 0:
                json_object = json.loads(message)
                msgtype = json_object['msgtype']
                if msgtype == "login":      # 상점 로그인 (실패시 소켓 끊김)
                    await self.login(json_object, websocket)
                elif msgtype == "genpin":   # PIN 번호 생성 요청
                    await self.genpin(json_object)
                elif msgtype == "order":    # 주문 처리 결과
                    await self.order(json_object)
                elif msgtype == "notify":   # Notification
                    await self.notify(json_object)
            elif len(message) == 0:
                print("message size = 0")
        except WebSocketDisconnect as e:
            logging.getLogger().error(e)
            # raise WebSocketDisconnect()
            raise
        except Exception as e:
            logging.getLogger().error(e)
            raise
            # raise Exception

    def disconnect(self, websocket):
        logging.getLogger().debug("ConnectionManager.disconnect")
        shop_no = 0
        for no, conn in self.connections:
            if conn == websocket:
                shop_no = no
                break
        del self.connections[shop_no]
        websocket.close()

    async def send(self, message: str, shop_no):
        logging.getLogger().debug("ConnectionManager.send")
        try:
            conn = self.connections.get(shop_no)
            await conn.send_text(message)
        except Exception as e:
            logging.getLogger().error(e)

    async def write(self, shop_no, message):
        logging.getLogger().debug("ConnectionManager.write")
        result = None
        try:
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
        from common import redis_pool
        try:
            shop_no = recvmsg['shop_no']
            while True:
                num = random.randrange(1, 9999)
                pin = str(num).zfill(4)
                if redis_pool.exists(pin) == 0:     # 중복 확인
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
            redis_pool.set(pin, data)

            conn = self.connections.get(str(shop_no))
            await conn.send_text(response)
        except Exception as e:
            logging.getLogger().error(e)

    async def order(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.order")
        shop_no = None
        try:
            shop_no = recvmsg['shop_no']
        except Exception as e:
            logging.getLogger().error(e)

    async def notify(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.notify")
        try:
            shop_no = recvmsg['shop_no']
            table_no = recvmsg['table_no']
        except Exception as e:
            logging.getLogger().error(e)

    async def send_order(self, params):
        result = False
        try:
            conn = self.connections.get(str(params['shop_no']))
            response = copy.deepcopy(params)
            response['msgtype'] = "order"
            response = str(json.dumps(response))
            await conn.send_text(response)
            result = True
        except Exception as e:
            logging.getLogger().error(f"send_order : {e}")
        return result
