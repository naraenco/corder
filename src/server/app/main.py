import logging
from util import rotation_log
from network.tornado_server import TornadoServer


if __name__ == "__main__":
    rotation_log.create_date_rotating_file_handler(
        log_name="corderserver",
        log_level=logging.DEBUG,
        console=True)
    logger = rotation_log.logging.getLogger()
    logger.info("------------------------------------------------------------")
    logger.info("COrder Server starting...")
    server = TornadoServer()
    server.run()
