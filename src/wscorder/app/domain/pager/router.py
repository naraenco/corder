import asyncio
import logging
import time
from fastapi import APIRouter
from .dto import PagerDto
from common import redis_pool, ws_manager
from common.util.corder_exception import CorderException
from common.util.response_entity import response_entity

router = APIRouter()
lock = asyncio.Lock()


@router.get("")
@router.get("", include_in_schema=False)
async def pagerget():
    success = True
    error = "0000"
    response = {"success": success, "error": error}
    return response


@router.post("/")
@router.post("", include_in_schema=False)
@response_entity
async def pagerpost(pagerdto: PagerDto):
    await lock.acquire()
    now = time
    pagerdto.regdate = now.strftime('%Y%m%d%H%M%S')
    data = pagerdto.dict()
    logging.getLogger().debug(f"pagerdto : {pagerdto}")

    error = "0000"
    try:
        shop_no = str(data['shop_no'])
        conn = await ws_manager.check_connection(shop_no)
        if conn is False:
            logging.getLogger().debug(f"orderpost : connetion not found")
            return "2001", data
        if redis_pool.exists(f"pin_{shop_no}_{data['otp_pin']}") == 0:
            return "1003", data
        error = await ws_manager.api_pager(data)
    except CorderException as e:
        logging.getLogger().debug(f"pagerpost 1 : {e}")
        # error = e.value()
    except Exception as e:
        logging.getLogger().debug(f"pagerpost 2 : {e}")
        error = "1001", data
    finally:
        lock.release()
    return error, data
