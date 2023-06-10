#include "json_util.h"
#include "strutil.h"
#include <iostream>

json_util::json_util()
{
    doc.SetObject();
}

json_util::~json_util()
{
}

void json_util::parse(const char *json)
{
    doc.Parse(json);
}

void json_util::add(const char *key, const char *value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        rapidjson::Value rjvalue(value, strlen(value), allocator);
        doc.AddMember(rjkey, rjvalue, allocator);
    }
    catch (...) {
    }
}

void json_util::add(const wchar_t *key, const wchar_t *value)
{
    try {
        string strkey = cbolt::strutil::wcs_to_utf8(key);
        string strvalue = cbolt::strutil::wcs_to_utf8(value);
        rapidjson::Value rjkey(strkey.c_str(), strkey.size(), allocator);
        rapidjson::Value rjvalue(strvalue.c_str(), strvalue.size(), allocator);
        doc.AddMember(rjkey, rjvalue, allocator);
    }
    catch (...) {
    }
}

void json_util::addl(const char *key, long value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        string strvalue = cbolt::strutil::long_to_str(value);
        rapidjson::Value rjvalue(strvalue.c_str(), strvalue.size(), allocator);
        doc.AddMember(rjkey, rjvalue, allocator);
    }
    catch (...) {
    }
}

void json_util::addl(const wchar_t *key, long value)
{
	try {
		string strkey = cbolt::strutil::wcs_to_utf8(key);
		string strvalue = cbolt::strutil::long_to_str(value);
		rapidjson::Value rjkey(strkey.c_str(), strkey.size(), allocator);
		rapidjson::Value rjvalue(strvalue.c_str(), strvalue.size(), allocator);
		doc.AddMember(rjkey, rjvalue, allocator);
	}
	catch (...) {
	}
}

void json_util::addb(const char *key, bool value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        string strvalue = "true";
        if (value == false) strvalue = "false";
        rapidjson::Value rjvalue(strvalue.c_str(), strvalue.size(), allocator);
        doc.AddMember(rjkey, rjvalue, allocator);
    }
    catch (...) {
    }
}

void json_util::add(const char *key, Value &value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        doc.AddMember(rjkey, value, allocator);
    }
    catch (...) {
    }
}

int json_util::get_int(const char *key)
{
    return doc.HasMember(key) ? doc[key].GetInt() : -1;
}

int json_util::get_int(const wchar_t *wkey)
{
    string key = cbolt::strutil::wcs_to_mbs(wkey);
    return doc.HasMember(key.c_str()) ? doc[key.c_str()].GetInt() : -1;
}

bool json_util::get_bool(const char *key)
{
    return doc.HasMember(key) ? doc[key].GetBool() : false;
}

bool json_util::get_bool(const wchar_t *wkey)
{
    string key = cbolt::strutil::wcs_to_mbs(wkey);
    return doc.HasMember(key.c_str()) ? doc[key.c_str()].GetBool() : false;
}

std::string json_util::get_string(const char *key)
{
    return doc.HasMember(key) ? string(doc[key].GetString()) : "";
}

std::wstring json_util::get_string(const wchar_t *wkey)
{
    string key = cbolt::strutil::wcs_to_mbs(wkey);

    string result = doc.HasMember(key.c_str()) ? string(doc[key.c_str()].GetString()) : "";
    return cbolt::strutil::mbs_to_wcs(result);
}

std::string json_util::str()
{
    try {
        StringBuffer strbuf;
        strbuf.Clear();
        Writer<StringBuffer> writer(strbuf);
        doc.Accept(writer);
        std::string result = strbuf.GetString();
        return result;
    }
    catch (...) {
        return "";
    }
}

