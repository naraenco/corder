import logging
import time
from fastapi import APIRouter
from database import SessionLocal
from common import redis_pool, ws_manager, logger
from common.util.corder_exception import CorderException
from common.util.response_entity import response_entity

router = APIRouter()


@router.get("/tablestatus/")
@router.get("/tablestatus", include_in_schema=False)
async def statusget():
    success = True
    error = "0000"
    response = {"success": success, "error": error}
    return response
