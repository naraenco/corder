from sqlalchemy import create_engine
from sqlalchemy.engine.url import URL
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from common import config

# SQLALCHEMY_DATABASE_URL = f"mysql+mysqldb://corder:dhejwnqns%211231@127.0.01:3306/corder"
url_object = URL.create(
    "mysql+mysqldb",
    username=config.get('db_uid'),
    password=config.get('db_pwd'),
    host=config.get('db_host'),
    port=config.get('db_port'),
    database=config.get('db_name'),
    query={'charset': 'utf8'}
)
engine = create_engine(url_object, pool_recycle=3600, connect_args={}, future=True)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()
