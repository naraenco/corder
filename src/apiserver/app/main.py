import os
import sys
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from fastapi import FastAPI
from starlette.middleware.cors import CORSMiddleware
from domain import root_router
import domain.order.router as order
from common import logger

origins = [
    "http://0.0.0.0:19000",
]

logger.info("--------------------------------------------------------------------------------")
logger.info("COrder - API Server 시작")

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
app.include_router(order.router, prefix='/api/order')

pid = os.getpid()
ppid = os.getppid()
logger.info(f"pid : {pid}, ppid : {ppid}")
