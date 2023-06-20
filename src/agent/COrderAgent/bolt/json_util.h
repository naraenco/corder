#pragma once
#include <string>
#include <fstream>
#include <boost/json.hpp>

class json_util
{
public:
    json_util();
    ~json_util();

    bool load(std::string path);
    bool parse(std::string data);
    bool write(std::string path, boost::json::value const& jv, bool pretty = false);
    void pretty_print(std::ostream& os, boost::json::value const& jv, std::string* indent = nullptr);
    boost::json::value& get() { return value; }

    int get_int(const char* key);
    bool get_bool(const char* key);
    std::string get_string(const char* key);
    std::string str();

private:
    boost::json::parse_options opt;
    boost::json::value value;
};
