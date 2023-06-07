import asyncio
import functools
import logging
import socket
import tornado.ioloop
from tornado.httpserver import HTTPServer
from tornado.web import Application
from util import config
from network.tcp_agent import AgentProtocol
from domain.root import RootHandler
from domain.auth.router import AuthHandler
from domain.menu.router import MenuHandler
from domain.order.router import OrderHandler


class TornadoServer:
    def __init__(self):
        self.logger = logging.getLogger()
        self.logger.info("init")
        self.connections = set()
        self.agent = AgentProtocol()
        self.port_api = config.get("port_api")
        self.port_agent = config.get("port_agent")

    async def main(self):
        self.logger.info("create server socket")
        sock_agent = socket.socket(socket.AF_INET, socket.SOCK_STREAM, 0)
        sock_agent.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        sock_agent.setblocking(False)
        sock_agent.bind(("", self.port_agent))
        sock_agent.listen(1000)

        callback_agent = functools.partial(self.agent.connection_ready, self.connections, sock_agent)

        app = Application([
            (r"/", RootHandler),
            (r"/auth", AuthHandler),
            (r"/menu", MenuHandler),
            (r"/order", OrderHandler)
        ])
        http_server = HTTPServer(app)
        http_server.listen(self.port_api)

        io_loop = tornado.ioloop.IOLoop.current()
        io_loop.add_handler(sock_agent.fileno(), callback_agent, io_loop.READ)

        await asyncio.Event().wait()

    def run(self):
        self.logger.info("run")
        asyncio.run(self.main())
