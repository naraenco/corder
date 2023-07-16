from sqlalchemy import Column, Integer, String
from database import Base


class OrderDao(Base):
    __tablename__ = "shop_order"

    order_no = Column(Integer, primary_key=True)
    shop_no = Column(Integer, nullable=False)
    table_cd = Column(String, nullable=False)
    otp_pin = Column(String, nullable=False)
    regdate = Column(String, nullable=False)
    pos_order = Column(String, nullable=True)
