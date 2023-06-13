#include "json_util.h"
#include "strutil.h"
#include <iostream>
#include "bolt/lib/rapidjson/error/error.h"
#include <boost/log/trivial.hpp>
#include <Windows.h>

json_util::json_util()
{
    doc.SetObject();
}

json_util::~json_util()
{
}

bool json_util::parse(const char* json)
{
    try {
        doc.Parse(json);
        if (doc.HasParseError()) {
            BOOST_LOG_TRIVIAL(error) << "json_util::parse - HasParseError : " << json;
            return false;
        }
        else {
            if (!doc.IsObject()) {
                BOOST_LOG_TRIVIAL(error) << "json_util::parse - isNotObject : " << json;
                return false;
            }
        }
    }
    catch (...) {
        ::OutputDebugStringA("json_util::parse exception");
        return false;
    }

    return true;
}

void json_util::add(const char* key, const char* value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        rapidjson::Value rjvalue(value, strlen(value), allocator);
        doc.AddMember(rjkey, rjvalue, allocator);
    }
    catch (...) {
    }
}

void json_util::add(const wchar_t* key, const wchar_t* value)
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

void json_util::addl(const char* key, long value)
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

void json_util::addl(const wchar_t* key, long value)
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

void json_util::addb(const char* key, bool value)
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

void json_util::add(const char* key, Value& value)
{
    try {
        rapidjson::Value rjkey(key, strlen(key), allocator);
        doc.AddMember(rjkey, value, allocator);
    }
    catch (...) {
    }
}

int json_util::get_int(const char* key)
{
    if (!doc.IsObject()) throw;
    try {
        return doc.HasMember(key) ? doc[key].GetInt() : -1;
    }
    catch (...) {
        throw;
    }
}

int json_util::get_int(const wchar_t* wkey)
{
    if (!doc.IsObject()) throw;
    string key = cbolt::strutil::wcs_to_mbs(wkey);
    try {
        return doc.HasMember(key.c_str()) ? doc[key.c_str()].GetInt() : -1;
    }
    catch (...) {
        throw;
    }
}

bool json_util::get_bool(const char* key)
{
    if (!doc.IsObject()) throw;
    try {
        return doc.HasMember(key) ? doc[key].GetBool() : false;
    }
    catch (...) {
        throw;
    }
}

bool json_util::get_bool(const wchar_t* wkey)
{
    if (!doc.IsObject()) throw;
    try {
        string key = cbolt::strutil::wcs_to_mbs(wkey);
        return doc.HasMember(key.c_str()) ? doc[key.c_str()].GetBool() : false;
    }
    catch (...) {
        throw;
    }
}

std::string json_util::get_string(const char* key)
{
    if (!doc.IsObject()) throw;
    try {
        return doc.HasMember(key) ? string(doc[key].GetString()) : "";
    }
    catch (...) {
        return NULL;
    }
}

std::wstring json_util::get_string(const wchar_t* wkey)
{
    if (!doc.IsObject()) throw;
    try {
        string key = cbolt::strutil::wcs_to_mbs(wkey);
        string result = doc.HasMember(key.c_str()) ? string(doc[key.c_str()].GetString()) : "";
        return cbolt::strutil::mbs_to_wcs(result);
    }
    catch (...) {
        throw;
    }
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

