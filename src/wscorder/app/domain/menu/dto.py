from pydantic import BaseModel


class MenuDto(BaseModel):
    shop_no: int
    table_cd: str
    otp_pin: str
    regdate: str = ''
    order_seq: int
