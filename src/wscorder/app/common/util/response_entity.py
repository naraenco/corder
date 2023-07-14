from functools import wraps


def response_entity(func):
    @wraps(func)
    async def wrapper(*args, **kwargs):
        # error, data = await func(*args, **kwargs)
        error = await func(*args, **kwargs)
        # print(kwargs)
        if error == "0000":
            success = True
        else:
            success = False
        # response = {"success": success, "error": error, "data": data}
        response = {"success": success, "error": error, "data": kwargs}
        return response
    return wrapper
