#include "json_file.h"
#include "lib/rapidjson/istreamwrapper.h"
#include "lib/rapidjson/prettywriter.h"
#include "lib/rapidjson/filereadstream.h"
#include <iostream>
#include "strutil.h"

namespace cbolt {

json_file::json_file()
{
    buffer = NULL;
    document = NULL;
}

json_file::~json_file()
{
    if (buffer)
    {
        delete[] buffer;
        buffer = NULL;
    }
    if (document)
    {
        delete document;
        document = NULL;
    }
}

bool json_file::load(const char *path)
{
    if (buffer) delete[] buffer;
    if (document) delete[] document;

    FILE *fp = fopen(path, "rb");
    fseek(fp, 0, SEEK_END);
    long filesize = ftell(fp);
    fseek(fp, 0, 0);
    if (filesize == 0) return false;
    buffer = new char[filesize + 1];
    FileReadStream bis(fp, buffer, filesize);
    AutoUTFInputStream<unsigned, FileReadStream> eis(bis);
    document = new Document();
    document->ParseStream<0, AutoUTF<unsigned> >(eis);
    fclose(fp);

    return true;
}

int json_file::get_int(const char *key)
{
    return document->HasMember(key) ? (*document)[key].GetInt() : -1; 
}

bool json_file::get_bool(const char *key)
{ 
    return document->HasMember(key) ? (*document)[key].GetBool() : false;
}

string json_file::get_string(const char *key)
{
    return document->HasMember(key) ? string((*document)[key].GetString()) : "";
}

wstring json_file::get_wstring(const char *key)
{
    string value = document->HasMember(key) ? string((*document)[key].GetString()) : "";
    return cbolt::strutil::mbs_to_wcs(value);
}

} // namespace cbolt
