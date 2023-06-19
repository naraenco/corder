#include "json_util.h"
#include <iostream>

json_util::json_util()
{
    opt.allow_invalid_utf8 = true;
}

json_util::~json_util()
{

}

bool json_util::load(std::string path)
{
    std::ifstream in(path);
    std::string buffer;

    if (!in.is_open()) {
        std::cout << "파일을 찾을 수 없습니다!" << std::endl;
        return false;
    }

    in.seekg(0, std::ios::end);
    int size = (int)in.tellg();
    buffer.resize(size);
    in.seekg(0, std::ios::beg);
    in.read(&buffer[0], size);

    value = boost::json::parse(buffer, boost::json::storage_ptr(), opt);
    return true;
}

bool json_util::parse(std::string data)
{
    value = boost::json::parse(data, boost::json::storage_ptr(), opt);
    return true;
}

bool json_util::write(std::string path, boost::json::value const& jv, bool pretty)
{
    std::ofstream os(path);

    if (pretty) {
        pretty_print(os, jv);
    }
    else {
        std::string text = boost::json::serialize(jv);
        os.write(text.c_str(), text.length());
    }
    os.close();
    return true;
}

void json_util::pretty_print(std::ostream& os, boost::json::value const& jv, std::string* indent)
{
    std::string indent_;
    if (!indent)
        indent = &indent_;
    switch (jv.kind())
    {
    case boost::json::kind::object:
    {
        os << "{\n";
        indent->append(4, ' ');
        auto const& obj = jv.get_object();
        if (!obj.empty())
        {
            auto it = obj.begin();
            for (;;)
            {
                os << *indent << boost::json::serialize(it->key()) << " : ";
                pretty_print(os, it->value(), indent);
                if (++it == obj.end())
                    break;
                os << ",\n";
            }
        }
        os << "\n";
        indent->resize(indent->size() - 4);
        os << *indent << "}";
        break;
    }

    case boost::json::kind::array:
    {
        os << "[\n";
        indent->append(4, ' ');
        auto const& arr = jv.get_array();
        if (!arr.empty())
        {
            auto it = arr.begin();
            for (;;)
            {
                os << *indent;
                pretty_print(os, *it, indent);
                if (++it == arr.end())
                    break;
                os << ",\n";
            }
        }
        os << "\n";
        indent->resize(indent->size() - 4);
        os << *indent << "]";
        break;
    }

    case boost::json::kind::string:
    {
        os << boost::json::serialize(jv.get_string());
        break;
    }

    case boost::json::kind::uint64:
        os << jv.get_uint64();
        break;

    case boost::json::kind::int64:
        os << jv.get_int64();
        break;

    case boost::json::kind::double_:
        os << jv.get_double();
        break;

    case boost::json::kind::bool_:
        if (jv.get_bool())
            os << "true";
        else
            os << "false";
        break;

    case boost::json::kind::null:
        os << "null";
        break;
    }

    if (indent->empty())
        os << "\n";
}

int json_util::get_int(const char* key)
{
    return boost::json::value_to<int>(value.at(key));
}

bool json_util::get_bool(const char* key)
{
    return boost::json::value_to<bool>(value.at(key));
}

std::string json_util::get_string(const char* key)
{
    return boost::json::value_to<std::string>(value.at(key));
}

std::string json_util::str()
{
    return boost::json::serialize(value);
}

