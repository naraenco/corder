import copy
import json
import logging
import random
import time
from sqlalchemy import text


def genpin(recvmsg):
    logging.getLogger().debug("ConnectionManager.genpin")
    try:
        from common import redis_pool
        shop_no = str(recvmsg['shop_no'])
        logging.getLogger().debug(f"ConnectionManager.genpin - shop_no : {shop_no}")
        while True:
            num = random.randrange(1, 9999)
            pin = str(num).zfill(4)
            if redis_pool.exists(f"pin_{shop_no}_{pin}") == 0:     # 중복 확인
                break
        now = time
        data = {
            "pin": pin,
            "created_at": now.strftime('%Y-%m-%d %H:%M:%S')
        }
        response = copy.deepcopy(data)
        response['msgtype'] = "genpin"
        response = str(json.dumps(response, ensure_ascii=False))
        data = str(json.dumps(data, ensure_ascii=False))
        logging.getLogger().debug(f"생성된 핀 데이터 : {data}")
        redis_pool.set(f"pin_{shop_no}_{pin}", data)
        return response
    except Exception as e:
        logging.getLogger().error(e)
    return None


async def update_menu(recvmsg):
    logging.getLogger().debug("ConnectionManager.menu")
    try:
        from database import engine
        sql = "SELECT COUNT(*) FROM data_menu WHERE shop_no=" + recvmsg['shop_no'] + ";"
        db = engine.connect()
        result = db.execute(text(sql)).fetchone()
        now = time
        regdate = now.strftime('%Y%m%d%H%M%S')
        data = json.dumps(recvmsg["data"]["PRODUCT"], ensure_ascii=False)
        touchclass = json.dumps(recvmsg["touch_class"]["TOUCHCLASS"], ensure_ascii=False)
        touchkey = json.dumps(recvmsg["touch_key"]["TOUCHKEY"], ensure_ascii=False)
        if result[0] == 0:
            sql = f"INSERT INTO data_menu (shop_no, regdate, touchclass, touchkey, data) VALUES (" \
                  f"{recvmsg['shop_no']}," \
                  f"'{regdate}'," \
                  f"'{touchclass}'," \
                  f"'{touchkey}'," \
                  f"'{data}')"
            db.execute(text(sql))
            db.commit()
        else:
            sql = f"UPDATE data_menu " \
                  f"SET regdate='{regdate}',touchclass='{touchclass}',touchkey='{touchkey}', data='{data}'" \
                  f"WHERE shop_no={recvmsg['shop_no']};"
            db.execute(text(sql))
            db.commit()
    except Exception as e:
        logging.getLogger().error(e)


async def update_tablemap(recvmsg):
    logging.getLogger().debug("ConnectionManager.tablemap")
    try:
        from database import engine
        sql = "SELECT COUNT(*) FROM data_table_map WHERE shop_no=" + recvmsg['shop_no'] + ";"
        db = engine.connect()
        result = db.execute(text(sql)).fetchone()
        now = time
        regdate = now.strftime('%Y%m%d%H%M%S')
        scdtable = json.dumps(recvmsg["scdtable"]["TABLE"], ensure_ascii=False)
        usetable = json.dumps(recvmsg["usetable"]["TABLEUSE"], ensure_ascii=False)
        if result[0] == 0:
            sql = f"INSERT INTO data_table_map (shop_no, regdate, scdtable, usetable) VALUES (" \
                  f"{recvmsg['shop_no']}," \
                  f"'{regdate}'," \
                  f"'{scdtable}'," \
                  f"'{usetable}')"
            db.execute(text(sql))
            db.commit()
        else:
            sql = f"UPDATE data_table_map " \
                  f"SET regdate='{regdate}',scdtable='{scdtable}',usetable='{usetable}'" \
                  f"WHERE shop_no={recvmsg['shop_no']};"
            db.execute(text(sql))
            db.commit()
    except Exception as e:
        logging.getLogger().error(e)
