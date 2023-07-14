import asyncio
import copy
import json
import logging
from fastapi import WebSocket, WebSocketDisconnect
from . import logic


class WebSocketManager:
    def __init__(self):
        self.connections: dict[str, WebSocket] = {}
        from common import redis_pool
        self.redis = redis_pool
        self.lock = asyncio.Lock()

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
                    response = logic.genpin(json_object)
                    conn = self.connections.get(str(json_object['shop_no']))
                    await conn.send_text(response)
                elif msgtype == "delpin":   # PIN 번호 삭제 요청
                    await self.delpin(json_object)
                elif msgtype == "tablestatus":
                    await self.tablestatus(json_object)
                elif msgtype == "tablemap":
                    await logic.update_tablemap(json_object)
                elif msgtype == "menu":
                    await logic.update_menu(json_object)
                elif msgtype == "clear":
                    await self.clear(json_object)

            elif len(message) == 0:
                print("message size = 0")
        except WebSocketDisconnect as e:
            logging.getLogger().error(e)
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

    # async def broadcast(self, message: str):
    #     logging.getLogger().debug("ConnectionManager.broadcast")
    #     for shop_no, conn in self.connections:
    #         await conn.send_text(message)

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

    async def delpin(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.delpin")
        try:
            shop_no = str(recvmsg['shop_no'])
            pin = f"pin_{shop_no}_{recvmsg['otp_pin']}"
            if self.redis.exists(pin) == 1:
                self.redis.remove(pin)
        except Exception as e:
            logging.getLogger().error(e)

    async def tablestatus(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.tablestatus")
        try:
            key = "status_" + str(recvmsg['shop_no'])
            data = recvmsg['data']
            data = json.dumps(data, ensure_ascii=False)
            self.redis.set(key, data)
        except Exception as e:
            logging.getLogger().error(e)

    async def clear(self, recvmsg):
        logging.getLogger().debug("ConnectionManager.clear")
        try:
            if recvmsg['type'] == 0:
                print("cancel")
            else:
                print("finish")
            shop_no = recvmsg['shop_no']
            table_cd = recvmsg['table_cd']
            key = f"order_{shop_no}_{table_cd}"
            orders = self.redis.get(key)
            orders = json.loads(orders)
            for order in orders:
                pin = order['pin']
                self.redis.remove(f"pin_{shop_no}_{pin}")
            self.redis.remove(key)
        except Exception as e:
            logging.getLogger().error(e)

    async def api_order(self,params):
        await self.lock.acquire()
        try:
            shop_no = str(params['shop_no'])
            table_cd = params['table_cd']
            pin = str(params['otp_pin'])
            conn = self.connections.get(shop_no)
            if conn is None:
                logging.getLogger().error("api_order - get connection failure")
                return "2001"
            response = copy.deepcopy(params)
            response['msgtype'] = "order"
            response['status'] = 0
            error = self.redis.update_pin(f"pin_{shop_no}_{pin}", table_cd)
            if error != "0000":
                logging.getLogger().error(f"api_order - update_pin failure code ({error})")
                return error
            key = f"order_{shop_no}_{table_cd}"
            orders = []
            current_order = {"order_no": params['order_no'], "pin": str(params['otp_pin'])}
            if self.redis.exists(key) == 0:
                orders.append(current_order)
            else:
                order_info = self.redis.get(key)
                orders = json.loads(order_info)
                orders.append(current_order)
            value = json.dumps(orders)
            self.redis.set(key, value)
            response = str(json.dumps(response, ensure_ascii=False))
            await conn.send_text(response)      # API에서 요청 받은 주문을 agent에 전송
        except Exception as e:
            error = "1001"
            logging.getLogger().error(f"api_order : {e}")
        self.lock.release()
        return error
