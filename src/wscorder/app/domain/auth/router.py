import json
from fastapi import APIRouter
from database import SessionLocal
from common import redis_pool, ws_manager, logger
from .dto import AuthDto
from .dao import AuthDao

router = APIRouter()


@router.get("/")
async def auth():
    return "auth"


@router.post("/")
async def auth(authdto: AuthDto):
    success = False
    error = "0000"
    data = authdto.dict()
    try:
        redis_data = json.loads(redis_pool.get(data['auth_key']))
        if redis_data is not None:
            if redis_data['shop_no'] == str(data['shop_no']):
                success = True
    except Exception as e:
        logger.error(e)

    response = {"success": success, "error": error}
    return response
