import json


class JsonConfig:
    def __init__(self):
        self.values = dict()

    def load(self, path):
        self.values.clear()
        try:
            with open(path, "r", encoding="utf-8") as _file:
                self.values = json.loads(_file.read())
        except IOError as ioe:
            print(ioe)
            raise

    def get(self, key1, key2=None):
        try:
            if key2 is None:
                return self.values[key1]
            return self.values[key1][key2]
        except KeyError:
            return None
