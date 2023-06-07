import os
import sys
sys.path.append(os.path.abspath(os.path.dirname(__file__)))
from util import logger
from network.tornado_server import TornadoServer


if __name__ == "__main__":
    logger.info("------------------------------------------------------------")
    logger.info("COrder Server starting...")
    server = TornadoServer()
    server.run()
