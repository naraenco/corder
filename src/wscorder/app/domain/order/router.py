import time
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
    print(f"orderpost : {data}")
    print(f"orderdto : {orderdto}")
    dao = OrderDao(**orderdto.dict())
    dao.regdate = time.time()
    db = SessionLocal()
    db.add(dao)
    db.commit()
    db.close()

    error = "0000"

    # try:
    #     shop_no = str(data['shop_no'])
    #     error = redis_pool.validate_pin(data['auth_key'], shop_no)
    #     orderno = await ws_manager.send_order(data)
    #     if orderno > 0:
    #         result = await ws_manager.wait_order(orderno)
    #         if result is True:
    #             success = True
    #             print(f"orderdto : {orderdto}")
    #             dao = OrderDao(**orderdto.dict())
    #             now = time
    #             dao.regdate = now.strftime('%Y%m%d%H%M%S')
    #             dao.regdate = time.time()
    #             db = SessionLocal()
    #             db.add(dao)
    #             db.commit()
    # except CorderException as e:
    #     error = e.value()
    # except Exception as e:
    #     logger.error(f"orderpost : {e}")
    #     success = False
    #     error = "1001"

    response = {"success": success, "error": error}
    print(f"response : {response}")
    return response
