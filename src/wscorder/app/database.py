from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

SQLALCHEMY_DATABASE_URL = "mysql+mysqldb://corder:dhejwnqns%211231@127.0.0.1:3306/corder"
# SQLALCHEMY_DATABASE_URL = "mysql+mysqldb://corder:dhejwnqns%211231@127.0.0.1:3306/corder?" \
#                           "allowPublicKeyRetrieval=true"
engine = create_engine(
    SQLALCHEMY_DATABASE_URL, pool_recycle=3600, connect_args={}
)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()
