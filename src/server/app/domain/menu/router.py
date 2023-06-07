import tornado


class MenuHandler(tornado.web.RequestHandler):
    def get(self):
        success = True
        error = "0000"
        response = {"success": success, "error": error}
        self.write(response)

    def post(self):
        success = True
        error = "0000"
        response = {"success": success, "error": error}
        self.write(response)
