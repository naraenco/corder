from fastapi import APIRouter

router = APIRouter()


@router.get("/")
def root():
    return {"service": "COrder Shop Server"}


@router.get("/version")
def version():
    return {"version": "2023.07.15 - build 01"}
