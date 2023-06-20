#define _WIN32_WINNT 0x0601

#include "network.h"
#include <boost/asio/streambuf.hpp> // ???
#include <boost/asio/buffer.hpp> // ???
#include <boost/asio.hpp>
#include <boost/log/trivial.hpp>
#include "constant.h"


void session::fail(beast::error_code ec, char const* what)
{
    //::OutputDebugStringA(ec.to_string().c_str());
    ::OutputDebugStringA(ec.message().c_str());
    //BOOST_LOG_TRIVIAL(error) << "session::fail - " << ec.to_string();
    BOOST_LOG_TRIVIAL(error) << "session::fail - " << ec.message();

    if (ec.value() == ERROR_CONNECTION_ABORTED) { // 1236
        connect_status = false;
        status_handler(ERROR_CONNECTION_ABORTED);
    }
    else if (ec.value() == WSAECONNABORTED) { // 10053
        connect_status = false;
        status_handler(WSAECONNABORTED);
    }
    else if (ec.value() == WSAECONNRESET) { // 10054
        connect_status = false;
        status_handler(WSAECONNRESET);
    }
    else if (ec.value() == WSAECONNREFUSED) { // 10061
        connect_status = false;
        status_handler(WSAECONNREFUSED);
    }
    else {
        connect_status = false; // 6?
        status_handler(ERROR_INVALID_HANDLE);
    }
    // WSAECONNRESET
    // WSAESHUTDOWN
}

void session::set_message_handler(func1 func)
{
    message_handler = func;
}

void session::set_status_hanlder(func2 func)
{
    status_handler = func;
}

void session::write(char const* text)
{
    ::OutputDebugString(L"session::write");

    if (connect_status == false) {
        ::OutputDebugString(L"session::on_write - Not Connected");
        return;
    }

    text_ = text;
    ws_.async_write(
        net::buffer(text_),
        beast::bind_front_handler(
            &session::on_write,
            shared_from_this()));
}

// Start the asynchronous operation
void session::start(char const* host, char const* port)
{
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
    ::OutputDebugString(L"session::on_connect");
    BOOST_LOG_TRIVIAL(info) << "session::on_connect";

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

    connect_status = true;

    // Perform the websocket handshake
    ws_.async_handshake(host_, "/ws",
        beast::bind_front_handler(
            &session::on_handshake,
            shared_from_this()));
}

void session::on_handshake(beast::error_code ec)
{
    ::OutputDebugString(L"session::on_handshake");
    BOOST_LOG_TRIVIAL(info) << "session::on_handshake";

    if (ec)
        return fail(ec, "handshake");

    status_handler(CONNECTION_SUCCESS);

    ws_.async_read(
        buffer_,
        beast::bind_front_handler(
            &session::on_read,
            shared_from_this()));
}

void session::on_write(beast::error_code ec, std::size_t bytes_transferred)
{
    ::OutputDebugString(L"session::on_write");

    boost::ignore_unused(bytes_transferred);

    if (ec)
        return fail(ec, "write");
}

void session::on_read(beast::error_code ec, std::size_t bytes_transferred)
{
    ::OutputDebugString(L"session::on_read");

    boost::ignore_unused(bytes_transferred);

    if (ec)
        return fail(ec, "read");

    std::string s(boost::asio::buffer_cast<const char*>(buffer_.data()), buffer_.size());
    //::OutputDebugStringA(s.c_str());
    BOOST_LOG_TRIVIAL(debug) << "session::on_read - " << s;
    message_handler(s);

    buffer_.consume(buffer_.size()); // clear the buffer

    ws_.async_read(
        buffer_,
        beast::bind_front_handler(
            &session::on_read,
            shared_from_this()));
}

void session::on_close(beast::error_code ec)
{
    ::OutputDebugString(L"session::on_close");
    BOOST_LOG_TRIVIAL(info) << "session::on_close";

    if (ec)
        return fail(ec, "close");

    // If we get here then the connection is closed gracefully

    // The make_printable() function helps print a ConstBufferSequence
    //std::cout << beast::make_printable(buffer_.data()) << std::endl;
}

