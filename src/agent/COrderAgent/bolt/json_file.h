#pragma once
#pragma warning(disable:4996)
#include <string>
#include "lib/rapidjson/document.h"

/*
주의 :
json_config 에서 사용할 config file 은 유니코드 형식 (UTF-8, UTF-16 등)으로 저장되어야 한다.
ASCII 형식을 사용할 경우, 영문자, 숫자 외의 문자 (한글 등)가 들어갈 경우 crash 될 수 있다.
*/

using namespace std;
using namespace rapidjson;

namespace cbolt {

class json_file
{
public:
    json_file();
    virtual ~json_file();
    Document& operator()() { return *document; }

    bool load(const char *path);
    bool parse(const char* json);
    int get_int(const char *key);
    bool get_bool(const char *key);
    string get_string(const char *key);
    wstring get_wstring(const char *key);
    Value::Object get_object(const char* key);
    Document* doc() { return document; }

private:
    Document *document;
};

}
