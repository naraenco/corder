from fastapi import APIRouter
from database import SessionLocal
from .dto import OrderDto
from .dao import OrderDao
from common import redis_pool, ws_manager, logger
from common.util.corder_exception import CorderException

router = APIRouter()


@router.get("/")
async def orderget():
    return "order"


@router.post("/")
async def orderpost(orderdto: OrderDto):
    success = False
    data = orderdto.dict()
    print(data)
    try:
        error = redis_pool.validate_pin(data['auth_key'], str(data['shop_no']))
        result = await ws_manager.send_order(data)
        if result is True:
            success = True
            # o = OrderDao()
            # db = SessionLocal()
            # db.add(o)
            # db.commit()
    except CorderException as e:
        error = e.value()
    except Exception as e:
        logger.error(f"orderpost : {e}")
        success = False
        error = "0001"

    print("error : ", error)
    response = {"success": success, "error": error}
    return response
