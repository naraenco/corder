from sqlalchemy import Column, Integer, String
from database import Base


class AuthDao(Base):
    __tablename__ = "test"

    no = Column(Integer, primary_key=True)
    name = Column(String, nullable=False)
    regist_at = Column(String, nullable=False)
