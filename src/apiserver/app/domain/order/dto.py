from pydantic import BaseModel


class OrderDto(BaseModel):
    shop_no: int
    table_cd: str
    otp_pin: str
    regdate: str = ''
    pos_order: dict
