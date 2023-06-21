from functools import wraps


def response_entity(func):
    @wraps(func)
    async def wrapper(*args, **kwargs):
        error = await func(*args, **kwargs)
        if error == "0000":
            success = True
        else:
            success = False
        response = {"success": success, "error": error}
        return response
    return wrapper