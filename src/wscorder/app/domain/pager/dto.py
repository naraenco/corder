from pydantic import BaseModel


class PagerDto(BaseModel):
    shop_no: int
    table_cd: str
    otp_pin: str
    regdate: str = ''
    desc: str
