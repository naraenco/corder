import asyncio
import os
import sys
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from starlette.middleware.cors import CORSMiddleware
from domain import root_router
import domain.menu.router as data
import domain.menu.router as menu
import domain.order.router as order
from common import ws_manager, logger

origins = [
    "http://0.0.0.0:19000",
    # "http://127.0.0.1:5173",
]

logger.info("--------------------------------------------------------------------------------")
logger.info("API Server 시작")

app = FastAPI()
app.router.redirect_slashes = False
app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
app.include_router(root_router.router, prefix='')
app.include_router(data.router, prefix='/api/data')
app.include_router(menu.router, prefix='/api/menu')
app.include_router(order.router, prefix='/api/order')


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
