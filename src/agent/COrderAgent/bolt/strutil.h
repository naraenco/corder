#pragma once
#include <string>
#include <iostream>
#include <vector>
#include <locale>

#define INVALID_SOURCE              -2
#define CANNOT_FIND_DELIMITER       -1

using namespace std;

class strutil {
public:
    strutil() {}
    ~strutil() {}

public:
    static void rtrim(string & value);
    static void rtrim(wstring & value);
    static void ltrim(string & value);
    static void ltrim(wstring & value);
    static void trim(string & value);
    static void trim(wstring & value);
    static string upper(const string & value);
    static wstring upper(const wstring & value);
    static string lower(const string & value);
    static wstring lower(const wstring & value);
    static int str_to_int(const char* value) { return atoi(value); }
    static int str_to_int(const wchar_t* value) { return _wtoi(value); }
    static int str_to_int(const string &value) { return atoi(value.c_str()); }
    static int str_to_int(const wstring &value) { return _wtoi(value.c_str()); }
    static long str_to_long(const char* value) { return atol(value); }
    static long str_to_long(const wchar_t* value) { return _wtol(value); }
    static long str_to_long(const string &value) { return atol(value.c_str()); }
    static long str_to_long(const wstring &value) { return _wtol(value.c_str()); }
    static long long str_to_longlong(const char* value) { return std::stoll(value); }
    static long long str_to_longlong(const wchar_t* value) { return std::stoll(value); }
    static long long str_to_longlong(const string &value) { return std::stoll(value); }
    static long long str_to_longlong(const wstring &value) { return std::stoll(value); }
    static string long_to_str(long value);
    static wstring long_to_wstr(long value);
    static string wcs_to_mbs(const wstring &value, locale const& loc = locale(""));
    static wstring mbs_to_wcs(const string &value, locale const& loc = locale(""));
    static string utf8_to_ansi(const string &value);
    static string ansi_to_utf8(const string &value);
    static wstring utf8_to_wcs(const string &value);
    static string wcs_to_utf8(const wstring &value);
    static int split(const string& str, vector<string>& vec, const string& delimiter);
    static int split(const wstring& str, vector<wstring>& vec, const wstring& delimiter);
    static int token(string& source, string& target, const string& delimiter);
    static int token(wstring& source, wstring& target, const wstring& delimiter);
    static string token(string& source, const string& delimiter);
    static wstring token(wstring& source, const wstring& delimiter);
    static string replaceall(const string& source, const string& pattern, const string& replace);
    static wstring replaceall(const wstring& source, const wstring& pattern, const wstring& replace);
    static bool compare_nocase(const string& s1, const string &s2) { return _stricmp(s1.c_str(), s2.c_str()) == 0 ? true : false; }
    static bool compare_nocase(const wstring& s1, const wstring &s2) { return _wcsicmp(s1.c_str(), s2.c_str()) == 0 ? true : false; }
    static int divide(string &strsur, string &strdest, char* cdelimiter);
    static int divide(string &strsur, string &strdest, char cdelimiter);
    static string divide(string &strsur, char* cdelimiter);
    static string divide(string &strsur, char cdelimiter);
    static wstring parse_string(wstring& value, wchar_t* pDeli);
    static string parse_string(string& value, char* pDeli);

    template <class T>
    void print_elements(const T& coll, const char* optcstr = "")
    {
        typename T::const_iterator pos;
        cout << optcstr << endl;
        for (pos = coll.begin(); pos != coll.end(); ++pos)
        {
            cout << *pos << endl;
        }
    }

    template <class T>
    void wprint_elements(const T& coll, const char* optcstr = "")
    {
        typename T::const_iterator pos;
        cout << optcstr << endl;
        for (pos = coll.begin(); pos != coll.end(); ++pos)
        {
            wcout << *pos << endl;
        }
    }
};
