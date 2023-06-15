from pydantic import BaseModel


class OrderDto(BaseModel):
    shop_no: int
    table_no: int
    auth_key: str
    regdate: str = ''
    pos_order: str
