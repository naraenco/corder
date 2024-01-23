import logging
import time
from fastapi import APIRouter
from .dto import PagerDto
from common import redis_pool
from common.util.response_entity import response_entity

router = APIRouter()


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
    now = time
    pagerdto.regdate = now.strftime('%Y%m%d%H%M%S')
    data = pagerdto.dict()
    logging.getLogger().debug(f"pagerdto : {pagerdto}")

    error = "0000"
    try:
        shop_no = str(data['shop_no'])
        if redis_pool.exists(f"pin_{shop_no}_{data['otp_pin']}") == 0:
            return "1003", data     # 존재하지 않는 PIN 번호 입니다

    except Exception as e:
        logging.getLogger().debug(f"pagerpost : {e}")
        error = "1001", data
    return error, data
