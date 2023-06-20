import time
from fastapi import APIRouter
from database import SessionLocal
from .dto import MenuDto
from common import redis_pool, ws_manager, logger
from common.util.corder_exception import CorderException

router = APIRouter()


@router.get("/")
async def menuget():
    success = True
    error = "0000"
    response = {"success": success, "error": error}
    return response


@router.post("/")
async def menupost(menudto: MenuDto):
    success = False
    data = menudto.dict()
    print(f"menudto : {menudto}")

    try:
        shop_no = str(data['shop_no'])
        error = redis_pool.validate_pin(data['otp_pin'], shop_no)
        if error == "0000":
            success = await ws_manager.query_menu(data)
    except CorderException as e:
        error = e.value()
    except Exception as e:
        logger.error(f"orderpost : {e}")
        success = False
        error = "1001"

    response = {"success": success, "error": error}
    print(f"response : {response}")
    return response
