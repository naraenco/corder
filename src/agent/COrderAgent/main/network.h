#pragma once

#include <boost/beast/core.hpp>
#include <boost/beast/websocket.hpp>
#include <boost/asio/strand.hpp>
#include <boost/asio.hpp>
#include <cstdlib>
#include <functional>
#include <iostream>
#include <memory>
#include <string>
#include <conio.h> 

namespace beast = boost::beast;         // from <boost/beast.hpp>
namespace http = beast::http;           // from <boost/beast/http.hpp>
namespace websocket = beast::websocket; // from <boost/beast/websocket.hpp>
namespace net = boost::asio;            // from <boost/asio.hpp>
using tcp = boost::asio::ip::tcp;       // from <boost/asio/ip/tcp.hpp>


class session : public std::enable_shared_from_this<session>
{
private:
    tcp::resolver resolver_;
    websocket::stream<beast::tcp_stream> ws_;
    beast::flat_buffer buffer_;
    std::string host_;
    std::string text_;
    typedef std::function<void(std::string)> func1;
    typedef std::function<void(int)> func2;
    func1 message_handler;
    func2 status_handler;
    bool connect_status;

public:
    explicit session(net::io_context& ioc)
        : resolver_(net::make_strand(ioc)),
        ws_(net::make_strand(ioc))
    {
        ::OutputDebugStringA("session::session()");
        connect_status = false;
    };

    ~session()
    {
        buffer_.clear();
    }

    void fail(beast::error_code ec, char const* what);
    void set_message_handler(func1 func);
    void set_status_hanlder(func2 func);

    void start(char const* host, char const* port);
    void write(char const* text);

    void on_resolve(beast::error_code ec, tcp::resolver::results_type results);
    void on_connect(beast::error_code ec, tcp::resolver::results_type::endpoint_type ep);
    void on_handshake(beast::error_code ec);
    void on_write(beast::error_code ec, std::size_t bytes_transferred);
    void on_read(beast::error_code ec, std::size_t bytes_transferred);
    void on_close(beast::error_code ec);
};
