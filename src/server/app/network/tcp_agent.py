import json
import logging
import random
import time
import tornado.ioloop
from tornado.iostream import IOStream
from tornado.iostream import StreamClosedError


class AgentProtocol:
    def __init__(self):
        self.logger = logging.getLogger()
        self.logger.info("init")

    async def handle_connection(self, conns, stream, address):
        while True:
            try:
                message = await stream.read_until(b"\n")
                self.logger.debug("message from agent {1} : {2}", address, message.decode().strip())
                json_object = json.loads(message)
                msgtype = json_object['msgtype']
                if msgtype == "login":
                    await self.login(json_object)
                elif msgtype == "genpin":
                    await self.genpin(json_object)
                elif msgtype == "order":    # 주문 처리 결과
                    await self.order(json_object)
                elif msgtype == "notify":   # Notification
                    await self.notify(json_object)
            except StreamClosedError as e:
                conns.remove(stream)
                self.logger.error(e)
                self.logger.info("StreamClosedError - connection count : {0}".format(len(conns)))
                break

    def connection_ready(self, conns, sock, fd, events):
        while True:
            try:
                connection, address = sock.accept()
            except BlockingIOError as e:
                logging.getLogger().error(e)
                return
            connection.setblocking(0)
            io_loop = tornado.ioloop.IOLoop.current()
            stream = IOStream(connection)
            conns.add(stream)
            self.logger.debug("connection count (Sync) : {0}".format(len(conns)))
            io_loop.spawn_callback(self.handle_connection, conns, stream, address)

    async def login(self, recvmsg):
        logging.getLogger().debug("AgentProtocol.login")
        print(recvmsg)

    async def genpin(self, recvmsg):
        logging.getLogger().debug("AgentProtocol.genpin")
        from util import redis_pool
        try:
            shop_no = recvmsg['shop_no']
            while True:
                num = random.randrange(1, 9999)
                pin = str(num).zfill(4)
                if redis_pool.exists(pin) == 0:     # redis에 pin이 존재하지 않음. 생성된 pin 사용
                    break

            now = time
            data = {
                "pin": pin,
                "shop_no": shop_no,
                "created_at": now.strftime('%Y-%m-%d %H:%M:%S')
            }
            data = str(json.dumps(data))
            self.logger.debug(f"생성된 핀 데이터 : {data}")
            redis_pool.set(pin, data)
            # await self.write(shop_no, data)
        except Exception as e:
            self.logger.error(e)

    async def order(self, recvmsg):
        logging.getLogger().debug("AgentProtocol.order")
        shop_no = None
        try:
            shop_no = recvmsg['shop_no']
        except Exception as e:
            logging.getLogger().error(e)

    async def notify(self, recvmsg):
        logging.getLogger().debug("AgentProtocol.notify")
        try:
            shop_no = recvmsg['shop_no']
            table_no = recvmsg['table_no']
        except Exception as e:
            logging.getLogger().error(e)
