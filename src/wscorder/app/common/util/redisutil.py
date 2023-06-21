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
            return "1002"
        return "0000"

    def update_pin(self, key, value):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                data = bytes(conn.get(key)).decode('utf-8')
                data = json.loads(data)
                print(f"update_pin #2 : {data}")
                if data.get('table_cd') is None:
                    data['table_cd'] = value
                elif data.get('table_cd') != value:
                    # 기존 사용한 TABLE 번호랑 PIN 번호가 다르다.
                    return "1006", 0
                seq = data.get('order_seq')
                if seq is None:
                    data['order_seq'] = 1
                else:
                    seq = seq + 1
                    data['order_seq'] = seq
                print(f"update_pin #3 : {data}")
                data = str(json.dumps(data))
                ttl = conn.ttl(key)
                conn.set(key, data)
                conn.expire(key, ttl)
        except Exception as e:
            logging.getLogger().error(e)
            return "1005", 0
        return "0000", seq
