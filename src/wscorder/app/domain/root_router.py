from fastapi import APIRouter

router = APIRouter()


@router.get("/")
def root():
    return {"service": "COrder"}


@router.get("/version")
def version():
    return {"version": "0.1"}
