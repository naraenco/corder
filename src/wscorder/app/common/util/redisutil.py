import json
import logging
import redis


class RedisUtil:
    def __init__(self):
        from common import config
        redisip = config.get('redis_ip')
        self.expire_time = int(config.get('expire_time')) * 60
        self.redis = redis.ConnectionPool(host=redisip,
                                          port=6379,
                                          db=0,
                                          max_connections=4,
                                          socket_connect_timeout=5,
                                          socket_timeout=5)
        self.expires: dict[str, int] = {}

    def set(self, key, value, shop_no=None):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                conn.set(key, value)
                if shop_no is None:
                    conn.expire(key, self.expire_time)
                else:
                    param = self.expires.get(shop_no)
                    conn.expire(key, param)
                    # logging.getLogger().debug(f"redis.set - shop_no: {shop_no}, expire_time: {param}")
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
                    # ttl = conn.ttl(key)
                    # logging.getLogger().debug(f"redis.get - {key}: {data} ({ttl})")
        except Exception as e:
            logging.getLogger().error(e)
            raise Exception
        return data

    def remove(self, key):
        data = None
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                if conn.exists(key) == 1:
                    conn.delete(key)
                    logging.getLogger().debug(f"redis.del - {key}")
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

    def validate_pin(self, key):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                if conn.exists(key) == 0:
                    return "1003"   # 존재하지 않는 PIN 번호 입니다
        except Exception as e:
            logging.getLogger().error(e)
            return "1002"   # PIN 인증 과정에서 오류가 발생했습니다
        return "0000"

    def update_pin(self, key, value):
        try:
            with redis.StrictRedis(connection_pool=self.redis) as conn:
                data = bytes(conn.get(key)).decode('utf-8')
                data = json.loads(data)
                if data.get('table_cd') is None:
                    data['table_cd'] = value
                elif data.get('table_cd') != value:     # 기존 사용한 TABLE 번호랑 PIN 번호가 다르다.
                    return "1006", 0
                data = str(json.dumps(data))
                ttl = conn.ttl(key)
                conn.set(key, data)
                conn.expire(key, ttl)
        except Exception as e:
            logging.getLogger().error(e)
            return "1005", 0    # PIN 번호 업데이트에 실패하였습니다
        return "0000"
