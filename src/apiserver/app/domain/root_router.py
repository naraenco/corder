from fastapi import APIRouter

router = APIRouter(redirect_slashes=False)


@router.get("/")
def root():
    return {"service": "COrder API Server"}


@router.get("/version")
def version():
    return {"version": "2024.01.23 - build 01"}
