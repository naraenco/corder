import json
import logging
import redis
from common.util.corder_exception import CorderException


class RedisUtil:
    def __init__(self):
        from common import config
        redisip = config.get('redis_ip')
        self.expire_time = config.get('pin_expire_time')
        self.redis = redis.ConnectionPool(host=redisip,
                                               port=6379,
                                               db=0,
                                               max_connections=4,
                                               socket_connect_timeout=5,
                                               socket_timeout=5)

    def set(self, key, value, expire_time=6000):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                conn.set(key, value)
                conn.expire(key, expire_time)
                logging.getLogger().debug(f"redis.set - {key}: {value}")
        except Exception as e:
            logging.getLogger().error(e)
            raise Exception

    def get(self, key):
        data = None
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                if conn.exists(key) == 1:
                    data = bytes(conn.get(key)).decode('utf-8')
                    ttl = conn.ttl(key)
                    logging.getLogger().debug(f"redis.get - {key}: {data} ({ttl})")
        except Exception as e:
            logging.getLogger().error(e)
            raise Exception
        return data

    def exists(self, key):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                return conn.exists(key)
        except Exception as e:
            logging.getLogger().error(e)
            raise Exception

    def validate_pin(self, key, value):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                if conn.exists(key) == 0:
                    return "1003"
                data = bytes(conn.get(key)).decode('utf-8')
                data = json.loads(data)
                if data['shop_no'] != value:
                    return "1004"
        except Exception as e:
            logging.getLogger().error(e)
            # raise CorderException("1002")
            return "1002"
        return "0000"
