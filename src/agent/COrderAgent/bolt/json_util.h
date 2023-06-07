#pragma once
#pragma warning(disable:4267)
#include "lib/rapidjson/document.h"
#include "lib/rapidjson/stringbuffer.h"
#include "lib/rapidjson/writer.h"
#include <string>

using namespace rapidjson;

class json_util
{
public:
    json_util();
    ~json_util();

public:
    void parse(const char *json);
    void add(const char *key, const char *value);
    void add(const wchar_t *key, const wchar_t *value);
	void add(const std::string key, const std::string value)	{ add(key.c_str(), value.c_str()); }
	void add(const std::wstring key, const std::wstring value)	{ add(key.c_str(), value.c_str()); }
    void addl(const char *key, long value);
	void addl(const wchar_t *key, long value);
    void addb(const char *key, bool value);
    void add(const char *key, Value &v);
    int get_int(const char *key);
    int get_int(const wchar_t *wkey);
    bool get_bool(const char *key);
    bool get_bool(const wchar_t *wkey);
    std::string get_string(const char *key);
    std::wstring get_string(const wchar_t *wkey);

    std::string str();

private:
    Document doc;
    Document::AllocatorType& allocator = doc.GetAllocator();
};
