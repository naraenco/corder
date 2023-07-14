import asyncio
import logging
import time
from fastapi import APIRouter
from database import SessionLocal
from .dto import OrderDto
from .dao import OrderDao
from common import redis_pool, ws_manager
from common.util.corder_exception import CorderException
from common.util.response_entity import response_entity

router = APIRouter()
lock = asyncio.Lock()


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
    await lock.acquire()
    now = time
    orderdto.regdate = now.strftime('%Y%m%d%H%M%S')
    data = orderdto.dict()
    logging.getLogger().debug(f"orderdto : {orderdto}")

    error = "0000"
    try:
        shop_no = str(data['shop_no'])
        conn = await ws_manager.check_connection(shop_no) 
        #if ws_manager.check_connection(shop_no) is False:
        if conn is False:
            logging.getLogger().debug(f"orderpost : connetion not found")
            return "2001"
        if redis_pool.exists(f"pin_{shop_no}_{data['otp_pin']}") == 0:
            return "1003"

        dao = OrderDao(**orderdto.dict())
        db = SessionLocal()
        db.add(dao)
        db.commit()
        data['order_no'] = dao.order_no
        # error = ws_manager.api_order(data)
        error = await ws_manager.api_order(data)
    except CorderException as e:
        logging.getLogger().debug(f"orderpost 1 : {e}")
        # error = e.value()
    except Exception as e:
        logging.getLogger().debug(f"orderpost 2 : {e}")
        error = "1001"
    finally:
        lock.release()
    return error, data
