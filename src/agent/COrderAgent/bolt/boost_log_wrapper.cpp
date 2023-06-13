#pragma warning(push)
#pragma warning(disable:4819)
#include <string>
#include "boost_log_wrapper.h"
#include <boost/date_time/posix_time/posix_time_types.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/console.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>
#include <boost/log/support/date_time.hpp>
#include <Windows.h>

#pragma warning(pop)
#pragma warning(disable:4503)

void init_boost_log()
{
    boost::log::add_common_attributes();

    auto fmtTimeStamp = boost::log::expressions::
        format_date_time<boost::posix_time::ptime>("TimeStamp", "%Y.%m.%d %H:%M:%S");
    boost::log::formatter logFmt =
        boost::log::expressions::format("[%1%] %2%")
        % fmtTimeStamp % boost::log::expressions::smessage;

    auto consoleSink = boost::log::add_console_log(std::clog);
    consoleSink->set_formatter(logFmt);

    char module[_MAX_PATH] = { 0, };
    GetModuleFileNameA(NULL, module, _MAX_PATH);
    std::string path = module;
    int pos = (int)path.find_last_of("\\");
    path = path.erase(pos + 1, path.length());
    path += "log\\%Y-%m-%d.log";
    
    auto fsSink = boost::log::add_file_log(
        boost::log::keywords::file_name = path,
        boost::log::keywords::rotation_size = 10 * 1024 * 1024,
        boost::log::keywords::min_free_space = 50 * 1024 * 1024,
        boost::log::keywords::time_based_rotation = boost::log::sinks::file::rotation_at_time_point(0, 0, 0),
        boost::log::keywords::auto_flush = true,
        boost::log::keywords::open_mode = std::ios_base::app);
    fsSink->set_formatter(logFmt);
    //fsSink->locked_backend()->auto_flush(true);
}
