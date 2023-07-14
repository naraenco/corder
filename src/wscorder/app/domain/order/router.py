import logging
import time
from fastapi import APIRouter
from database import SessionLocal
from .dto import OrderDto
from .dao import OrderDao
from common import redis_pool, ws_manager, logger
from common.util.corder_exception import CorderException
from common.util.response_entity import response_entity

router = APIRouter()


@router.get("")
@router.get("", include_in_schema=False)
async def orderget():
    success = True
    error = "0000"
    response = {"success": success, "error": error}
    return response


@router.post("/")
@router.post("", include_in_schema=False)
@response_entity
async def orderpost(orderdto: OrderDto):
    now = time
    orderdto.regdate = now.strftime('%Y%m%d%H%M%S')
    data = orderdto.dict()
    logging.getLogger().debug(f"orderdto : {orderdto}")

    error = "0000"
    try:
        shop_no = str(data['shop_no'])
        if ws_manager.check_connection(shop_no) is not True:
            return "2001"
        if redis_pool.exists(f"pin_{shop_no}_{data['otp_pin']}") == 0:
            return "1003"

        dao = OrderDao(**orderdto.dict())
        db = SessionLocal()
        db.add(dao)
        db.commit()
        data['order_no'] = dao.order_no
        error = await ws_manager.api_order(data)
    except CorderException as e:
        logger.error(f"orderpost : {e}")
        # error = e.value()
    except Exception as e:
        logger.error(f"orderpost : {e}")
        error = "1001"
    return error, data

