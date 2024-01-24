import asyncio
import os
import sys

import common

sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from starlette.middleware.cors import CORSMiddleware
from domain import root_router
from common import ws_manager, logger
import subscribe

origins = [
    "http://0.0.0.0:19001",
]
app = FastAPI()


@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    try:
        while True:
            message_task = asyncio.create_task(ws_manager.message_handler(websocket))
            done, pending = await asyncio.wait(
                {message_task},
                return_when=asyncio.FIRST_COMPLETED,
            )
            for task in pending:
                task.cancel()
            for task in done:
                task.result()
    except WebSocketDisconnect as e:
        logger.error(f"WebSocketDisconnect : {e}")
    except Exception as e:
        logger.error(f"Exception : {e}")


logger.info("--------------------------------------------------------------------------------")
logger.info("COrder - Shop Server 시작")
logger.info(f"pid : {os.getpid()}, ppid : {os.getppid()}")
try:
    app.router.redirect_slashes = False
    app.add_middleware(
        CORSMiddleware,
        allow_origins=origins,
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )
    app.include_router(root_router.router, prefix='')
    subscribe.run()

    common.redis_pool.publish("corder", "message1")

except (KeyboardInterrupt, SystemExit):
    sys.exit()
