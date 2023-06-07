from pydantic import BaseModel


class AuthDto(BaseModel):
    shop_no: int
    table_no: int
    auth_key: str
