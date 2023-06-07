from fastapi import APIRouter

router = APIRouter()


@router.get("/")
async def menu(shop_no=0):
    success = dict()
    error = "0000"
    result = {"menus": success, "error": error}
    return result
