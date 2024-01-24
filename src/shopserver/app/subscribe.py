import sys
from threading import Thread
from common import redis_pool

channel_name = 'corder'


def subscribe():
    pubsub = redis_pool.pubsub()
    pubsub.subscribe('corder')
    for message in pubsub.listen():
        print(message)


def run():
    try:
        sub_thread = Thread(target=subscribe, daemon=True)
        sub_thread.start()
    except (KeyboardInterrupt, SystemExit):
        sys.exit()
