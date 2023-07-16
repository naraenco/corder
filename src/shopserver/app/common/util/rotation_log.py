import codecs
import logging
import os
import sys
import time
from pathlib import Path
from logging.handlers import TimedRotatingFileHandler


class DateRotatingFileHandler(TimedRotatingFileHandler):
    def __init__(self, path, log_name, ext):
        self.path = path
        self.log_name = log_name
        self.ext = ext

        self.baseFilename = self.get_filename()
        logging.handlers.TimedRotatingFileHandler.__init__(self,
                                                           self.baseFilename,
                                                           when='midnight',
                                                           interval=1,
                                                           backupCount=4,
                                                           encoding='utf-8')

    def doRollover(self):
        self.stream.close()
        self.baseFilename = self.get_filename()
        if self.encoding:
            self.stream = codecs.open(self.baseFilename, 'a', self.encoding)
        else:
            self.stream = open(self.baseFilename, 'a')

    def get_filename(self):
        return "%s/%s-%s.%s" % (self.path, time.strftime("%Y%m%d"), self.log_name, self.ext)


def create_date_rotating_file_handler(
        log_level=None,
        log_path=None,
        log_name=None,
        log_format=None,
        console=False,
        ext='log'):

    level = log_level or logging.DEBUG
    # path = log_path or os.path.dirname(os.path.abspath(__file__))

    path = log_path or os.getcwd()
    path = str(Path(path).parent) + "/log"
    create_directory(path)
    lformat = log_format or logging.Formatter('%(asctime)s:%(module)s:%(levelname)s - %(message)s')
    name = log_name or os.path.basename(sys.argv[0]).replace(".py", "")

    handler = DateRotatingFileHandler(path, name, ext)
    handler.setFormatter(lformat)

    logger = logging.getLogger('gunicorn.error')
    logger.addHandler(handler)
    logger.setLevel(level)

    if console:
        console_handler = logging.StreamHandler(sys.stdout)
        console_handler.setFormatter(lformat)
        logger.addHandler(console_handler)


def create_directory(directory):
    try:
        if not os.path.exists(directory):
            os.makedirs(directory)
    except OSError:
        print("Error: Failed to create the directory.")
