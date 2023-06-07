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
                msgcls = json_object['class']
                if msgcls == "login":
                    self.login(json_object)
                elif msgcls == "genotp":
                    self.genotp(json_object)
            except StreamClosedError as e:
                logging.getLogger().error(e)
                conns.remove(stream)
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

    def login(self, recvmsg):
        print(recvmsg)

    def genotp(self, recvmsg):
        print(recvmsg)
        shop_no = None
        otp = None
        try:
            shop_no = recvmsg['shop_no']
        except Exception as e:
            logging.getLogger().error(e)

        while True:
            num = random.randrange(1, 999999)
            otp = str(num).zfill(6)
            print(otp)
            # 6자리 OTP PIN 번호 생성 (redis에 중복이 있는지 확인 필요)
            break
        now = time
        data = {
            "shop_no": shop_no,
            "created_at": now.strftime('%Y-%m-%d %H:%M:%S')
        }
        data = str(json.dumps(data))
        print(data)
        # redis에 data저장 (expired time : 2시간)
