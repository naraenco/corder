#include "network.h"

#include <boost/asio/streambuf.hpp> // ???
#include <boost/asio/buffer.hpp> // ???
#include <boost/asio.hpp>
#include "../bolt/json_util.h"

// Report a failure
void fail(beast::error_code ec, char const* what)
{
    std::cerr << what << ": " << ec.message() << "\n";
    ::OutputDebugStringA(ec.message().c_str());
}

session::~session()
{
    ::OutputDebugStringA("session::~session()");
    ws_.close(websocket::close_code::normal);
    buffer_.clear();
}

void session::write(char const* text)
{
    std::cout << "write" << std::endl;
    ::OutputDebugString(L"session::write");

    text_ = text;
    //::OutputDebugStringA(text_.c_str());

    ws_.async_write(
        net::buffer(text_),
        beast::bind_front_handler(
            &session::on_write,
            shared_from_this()));
}

// Start the asynchronous operation
void session::start(char const* host, char const* port)
{
    std::cout << "start" << std::endl;
    ::OutputDebugString(L"start");

    // Save these for later
    host_ = host;

    // Look up the domain name
    resolver_.async_resolve(
        host,
        port,
        beast::bind_front_handler(
            &session::on_resolve,
            shared_from_this()));
}

void session::on_resolve(beast::error_code ec, tcp::resolver::results_type results)
{
    std::cout << "on_resolve" << std::endl;
    ::OutputDebugString(L"on_resolve");

    if (ec)
        return fail(ec, "resolve");

    // Set the timeout for the operation
    beast::get_lowest_layer(ws_).expires_after(std::chrono::seconds(30));

    // Make the connection on the IP address we get from a lookup
    beast::get_lowest_layer(ws_).async_connect(
        results,
        beast::bind_front_handler(
            &session::on_connect,
            shared_from_this()));
}

void session::on_connect(beast::error_code ec, tcp::resolver::results_type::endpoint_type ep)
{
    std::cout << "on_connect" << std::endl;
    ::OutputDebugString(L"on_connect");

    if (ec)
        return fail(ec, "connect");

    // Turn off the timeout on the tcp_stream, because
    // the websocket stream has its own timeout system.
    beast::get_lowest_layer(ws_).expires_never();

    // Set suggested timeout settings for the websocket
    //ws_.set_option(
    //    websocket::stream_base::timeout::suggested(
    //        beast::role_type::client));

    websocket::stream_base::timeout timeoutOpt{
        //std::chrono::seconds(30),   // handshake timeout
        //std::chrono::seconds(20),       // idle timeout. Any ways to set the ping interval as well?
        boost::beast::websocket::stream_base::none(),
        boost::beast::websocket::stream_base::none(),
        true   //enable ping-pong to keep alive
    };
    boost::asio::socket_base::keep_alive option(true);
    
    ws_.set_option(timeoutOpt);

    // Set a decorator to change the User-Agent of the handshake
    ws_.set_option(websocket::stream_base::decorator(
        [](websocket::request_type& req)
        {
            req.set(http::field::user_agent,
            std::string(BOOST_BEAST_VERSION_STRING) +
            " websocket-client-async");
        }));

    // Update the host_ string. This will provide the value of the
    // Host HTTP header during the WebSocket handshake.
    // See https://tools.ietf.org/html/rfc7230#section-5.4
    host_ += ':' + std::to_string(ep.port());

    // Perform the websocket handshake
    ws_.async_handshake(host_, "/ws",
        beast::bind_front_handler(
            &session::on_handshake,
            shared_from_this()));
}

void session::on_handshake(beast::error_code ec)
{
    std::cout << "on_handshake" << std::endl;
    ::OutputDebugString(L"on_handshake");

    if (ec)
        return fail(ec, "handshake");

    ws_.async_read(
        buffer_,
        beast::bind_front_handler(
            &session::on_read,
            shared_from_this()));
}

void session::on_write(beast::error_code ec, std::size_t bytes_transferred)
{
    std::cout << "on_write" << std::endl;
    ::OutputDebugString(L"on_write");

    boost::ignore_unused(bytes_transferred);

    if (ec)
        return fail(ec, "write");
}

void session::on_read(beast::error_code ec, std::size_t bytes_transferred)
{
    std::cout << "on_read" << std::endl;
    ::OutputDebugString(L"on_read");

    boost::ignore_unused(bytes_transferred);

    if (ec)
        return fail(ec, "read");

    std::string s(boost::asio::buffer_cast<const char*>(buffer_.data()), buffer_.size());
    handle_message(s);

    //std::cout << beast::make_printable(buffer_.data()) << std::endl;

    buffer_.consume(buffer_.size()); // Clear the buffer

    ws_.async_read(
        buffer_,
        beast::bind_front_handler(
            &session::on_read,
            shared_from_this()));
}

void session::on_close(beast::error_code ec)
{
    std::cout << "on_close" << std::endl;
    ::OutputDebugString(L"on_close");

    if (ec)
        return fail(ec, "close");

    // If we get here then the connection is closed gracefully

    // The make_printable() function helps print a ConstBufferSequence
    std::cout << beast::make_printable(buffer_.data()) << std::endl;
}

void session::handle_message(std::string s)
{
    ::OutputDebugString(L"handle_message");
    ::OutputDebugStringA(s.c_str());

    try {
        json_util util;
        util.parse(s.c_str());
        std::string msgtype = util.get_string("msgtype");

        ::OutputDebugStringA(msgtype.c_str());

        if (msgtype == "genpin") {
            ::OutputDebugStringA(util.get_string("pin").c_str());
        }
        else if (msgtype == "order") {

        }
        else if (msgtype == "login") {
        }
        else {
            ::OutputDebugString(L"Unknown Message Type");
        }
    }
    catch(...) {
        ::OutputDebugStringA("handle_message exception");
    }
}
