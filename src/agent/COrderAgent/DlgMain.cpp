#include "stdafx.h"
#include <ShellAPI.h>
#include <functional>
#include <thread>
#include "bolt/boost_log_wrapper.h"
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/core.hpp>
#include <boost/log/trivial.hpp>
#include <boost/log/expressions.hpp>
#include "resource.h"
#include "MFCUIDoc.h"
#include "MFCUIView.h"
#include "MainFrm.h"
#include "AfxDialogEx.h"
#include "DlgMain.h"
#include "DlgAppInfo.h"
#include "common/CommonData.h"
#include "bolt/strutil.h"
#include "lib/system.h"
#include "main/constant.h"
#include "main/corder_config.h"
#include "main/network.h"


corder_config* corder_config::instance_ = nullptr;
net::io_context ioc;
std::shared_ptr<session> ws_session;
boost::thread session_thread;
boost::thread connect_thread;

static std::string server_address = "127.0.0.1";
static std::string server_port = "19000";

static void service()
{
    try {
        ws_session->start(server_address.c_str(), server_port.c_str());
    }
    catch (...) {

    }
}

//void run()
//{
//    ws_session = std::make_shared<session>(ioc);
//    session_thread = boost::thread(boost::bind(&boost::asio::io_service::run, &ioc));
//    ioc.post(boost::bind(&service));
//}


IMPLEMENT_DYNAMIC(CDlgMain, CDialogEx)

CDlgMain::CDlgMain(CWnd* pParent /*=NULL*/)
    : CDialogEx(CDlgMain::IDD, pParent)
{
    m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
    pMFCUIView = (CMFCUIView*)pParent;

    bRelease = false;
    bManager = true;
    bConnect = false;
    log_no = 0;
}

CDlgMain::~CDlgMain()
{
}

void CDlgMain::DoDataExchange(CDataExchange* pDX)
{
    CDialogEx::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_LIST_MAIN, m_list_main);
}

BEGIN_MESSAGE_MAP(CDlgMain, CDialogEx)
    ON_WM_CTLCOLOR()
    ON_WM_PAINT()
    ON_WM_QUERYDRAGICON()
    ON_WM_LBUTTONDOWN()
    ON_WM_LBUTTONUP()
    ON_WM_KEYDOWN()
    ON_WM_MOUSEMOVE()
    ON_COMMAND(IDC_ABOUT, &CDlgMain::About)
    ON_COMMAND(IDC_EXIT, &CDlgMain::Exit)
    ON_WM_CLOSE()
    ON_WM_DESTROY()
    ON_WM_DROPFILES()
END_MESSAGE_MAP()


BOOL CDlgMain::Create(CRect& wndRect, UINT nIDTemplate, CWnd* pParentWnd)
{
    m_WindowRect.CopyRect(&wndRect);
    return CDialogEx::Create(nIDTemplate, pParentWnd);
}

BOOL CDlgMain::PreTranslateMessage(MSG* pMsg)
{
    switch (pMsg->wParam)
    {
    case VK_ESCAPE:
        return TRUE;

    case VK_RETURN:
        return TRUE;

    default:
        break;
    }

    switch (pMsg->message)
    {
    case WM_MOUSEMOVE:
        break;
    }

    return CDialogEx::PreTranslateMessage(pMsg);
}

HCURSOR CDlgMain::OnQueryDragIcon()
{
    return (HCURSOR)m_hIcon;
}

void CDlgMain::ComponentResize()
{
    CRect rect;
    GetClientRect(&rect);
    nListWidth = rect.right - 250;
    nListHeight = rect.bottom - 85;
    nListX = 30;
    nListY = 20;
    m_list_main.MoveWindow(nListX, nListY, nListWidth, rect.bottom - 103, TRUE);
    m_list_main.SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
}

void CDlgMain::WriteLog(std::string value)
{
    log_no++;
    wstring no = strutil::long_to_wstr(log_no);
    m_list_main.InsertItem(0, no.c_str());
    m_list_main.SetItemText(0, 1, System::get_datetimew().c_str());
    m_list_main.SetItemText(0, 2, strutil::mbs_to_wcs(value).c_str());
}

void CDlgMain::WriteLog(std::wstring value)
{
    log_no++;
    wstring no = strutil::long_to_wstr(log_no);
    m_list_main.InsertItem(0, no.c_str());
    m_list_main.SetItemText(0, 1, System::get_datetimew().c_str());
    m_list_main.SetItemText(0, 2, value.c_str());
}

BOOL CDlgMain::OnInitDialog()
{
    CDialogEx::OnInitDialog();

    MoveWindow(m_WindowRect.left, m_WindowRect.top, m_WindowRect.right - m_WindowRect.left, m_WindowRect.bottom - m_WindowRect.top, TRUE);

    m_list_main.InsertColumn(0, _T("No"), LVCFMT_LEFT, 0);
    m_list_main.InsertColumn(1, _T("DateTime"), LVCFMT_LEFT, 150);
    m_list_main.InsertColumn(2, _T("Log"), LVCFMT_LEFT, 350);
    m_list_main.ShowWindow(SW_SHOW);

    ComponentResize();
    
    char module[_MAX_PATH] = { 0, };
    GetModuleFileNameA(NULL, module, _MAX_PATH);
    string path = module;
    int pos = (int)path.find_last_of("\\");
    path = path.erase(pos + 1, path.length());
    path += "config.json";

    if (boost::filesystem::exists(path)) {
        corder_config::instance()->get_config()->load(path.c_str());
        pos_extra = corder_config::get_string("pos_extra") + ".json";
        path_order = corder_config::get_string("path_order") + "Order_";
        shop_no = corder_config::get_string("shop_no");
        auth_key = corder_config::get_string("auth_key");
        print_port = corder_config::get_string("print_port");
        print_font_width = corder_config::get_int("print_font_width");
        print_font_height = corder_config::get_int("print_font_height");

        server_address = corder_config::get_string("server_address");
        server_port = corder_config::get_string("server_port");
    }
    else {
        ::OutputDebugString(L"<config.json> file not found");
    }

    init_boost_log();

    reconnect_time = corder_config::instance()->get_int("timer_connect_retry") * 1000;

    if (corder_config::get_string("log_level") == "debug") {
        boost::log::core::get()->set_filter(boost::log::trivial::severity >= boost::log::trivial::debug);
    }
    else {
        boost::log::core::get()->set_filter(boost::log::trivial::severity >= boost::log::trivial::info);
    }

    BOOST_LOG_TRIVIAL(info) << "========================== COrder Agent Start ==========================";

    connect_thread = boost::thread(&CDlgMain::ConnectionManager, this);

    return TRUE;
}

void CDlgMain::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
    switch (nChar)
    {
    case VK_ESCAPE:
        break;

    case VK_RETURN:
        break;
    }
    CDialogEx::OnKeyDown(nChar, nRepCnt, nFlags);
}

HBRUSH CDlgMain::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    HBRUSH hbr = CDialogEx::OnCtlColor(pDC, pWnd, nCtlColor);

    switch (nCtlColor)
    {
    case CTLCOLOR_DLG:
        return (HBRUSH)GetStockObject(WHITE_BRUSH);
        break;
    case CTLCOLOR_STATIC:
        //if (pWnd->m_hWnd == m_static_filename.m_hWnd)
        //{
        //    SetTextColor(pDC->m_hDC, RGB(35, 129, 189));
        //}
        SetBkColor(pDC->m_hDC, RGB(255, 255, 255));
        return (HBRUSH)GetStockObject(WHITE_BRUSH);
        break;
    }
    return hbr;
}

void CDlgMain::Resize(CRect& rect)
{
    m_WindowRect.CopyRect(&rect);
    MoveWindow(m_WindowRect.left, m_WindowRect.top, m_WindowRect.right - m_WindowRect.left, m_WindowRect.bottom - m_WindowRect.top, TRUE);
    ComponentResize();
    Invalidate();
}

void CDlgMain::OnPaint()
{
    CPaintDC dc(this);
    CRect rcWindow;
    GetClientRect(&rcWindow);

    CPen pen, * pOldPen;
    pen.CreatePen(PS_SOLID, 1, RGB(153, 210, 230));
    pOldPen = (CPen*)dc.SelectObject(&pen);
    dc.RoundRect(rcWindow.left, rcWindow.top, rcWindow.right, rcWindow.bottom, 20, 20);
    dc.RoundRect(rcWindow.left + 1, rcWindow.top + 1, rcWindow.right - 1, rcWindow.bottom - 1, 20, 20);
    dc.SelectObject(pOldPen);
    DeleteObject(pen);

    Gdiplus::Graphics graphics(dc);

    // convert
    int nServiceX = rcWindow.right - 200;
    int nServiceY = rcWindow.bottom - 190;
    int nServiceSpace = 20;

    commonData.m_icon_genpin.Load(IDB_PNG_PINGEN, _T("PNG"), AfxGetApp()->m_hInstance);
    CommonData::draw(graphics, commonData.m_icon_genpin, nServiceX, nServiceY, nServiceSpace, rcIconGenPin);

    // function
    int nFunctionX = nListX + nListWidth - (32 * 4) - 60;
    int nFunctionY = nListY + nListHeight + 10;
    int nFunctionSpace = 20;

    //CommonData::draw(graphics, commonData.m_icon_document, nFunctionX, nFunctionY, nFunctionSpace, rcIconDocument);
    //CommonData::draw(graphics, commonData.m_icon_folderdoc, nFunctionX, nFunctionY, nFunctionSpace, rcIconFolderDoc);
    //CommonData::draw(graphics, commonData.m_icon_trash, nFunctionX, nFunctionY, nFunctionSpace, rcIconTrash);
    //CommonData::draw(graphics, commonData.m_icon_refresh, nFunctionX, nFunctionY, nFunctionSpace, rcRefresh);

    // etc
    commonData.m_image_body.Load(IDB_PNG_TEXT, _T("PNG"), AfxGetApp()->m_hInstance);
    graphics.DrawImage(commonData.m_image_body,
        rcWindow.right - (commonData.m_image_body.m_pBitmap->GetWidth() + 10),
        rcWindow.top + 20,
        commonData.m_image_body.m_pBitmap->GetWidth(), commonData.m_image_body.m_pBitmap->GetHeight());
}

void CDlgMain::OnLButtonDown(UINT nFlags, CPoint point)
{
    if (IsPosition(rcIconGenPin, point))
    {
        GenPin();
    }
    else if (IsPosition(rcIconDocument, point))
    {
    }
    else if (IsPosition(rcIconFolderDoc, point))
    {
    }
    else if (IsPosition(rcIconTrash, point))
    {
        //if (m_list_main.GetItemCount() == 0) return;
        //if (MessageBox(_T("All lists are deleting.\nWould you like to process?"), _T("Bolt Text Manager"), MB_YESNO) == IDYES)
        //{
        //    m_list_main.DeleteAllItems();
        //}
    }
    else if (IsPosition(rcRefresh, point))
    {
    }
    else
    {
        GetParentFrame()->PostMessage(WM_NCLBUTTONDOWN, HTCAPTION, MAKELPARAM(point.x, point.y));
    }

    CDialogEx::OnLButtonDown(nFlags, point);
}


void CDlgMain::OnLButtonUp(UINT nFlags, CPoint point)
{
    CDialogEx::OnLButtonUp(nFlags, point);
}

void CDlgMain::OnMouseMove(UINT nFlags, CPoint point)
{
    if (IsPosition(rcIconGenPin, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
    //else if (IsPosition(rcIconDocument, point))
    //{
    //    SetCursor(LoadCursor(NULL, IDC_HAND));
    //}
    //else if (IsPosition(rcIconFolderDoc, point))
    //{
    //    SetCursor(LoadCursor(NULL, IDC_HAND));
    //}
    //else if (IsPosition(rcIconTrash, point))
    //{
    //    SetCursor(LoadCursor(NULL, IDC_HAND));
    //}
    //else if (IsPosition(rcRefresh, point))
    //{
    //    SetCursor(LoadCursor(NULL, IDC_HAND));
    //}
    else
    {
        SetCursor(LoadCursor(NULL, IDC_ARROW));
    }
    CDialogEx::OnMouseMove(nFlags, point);
}

void CDlgMain::Config()
{
}

void CDlgMain::About()
{
    CDlgAppInfo dlgAppInfo;
    dlgAppInfo.DoModal();
}

void CDlgMain::Exit()
{
    bManager = false;
    pMFCUIView->Exit();
}

void CDlgMain::Connect()
{
}

void CDlgMain::Login()
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::Login()";
    ::OutputDebugStringA("CDlgMain::Login()");
    std::string message = "{\"msgtype\":\"login\",\"shop_no\": \"" 
        + shop_no + "\",\"auth_key\":\"" 
        + auth_key + "\"}";
    ws_session->write(message.c_str());
}

void CDlgMain::GenPin()
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::GenPin()";
    ::OutputDebugStringA("CDlgMain::GenPin()");
    std::string message = "{\"msgtype\":\"genpin\",\"shop_no\": \"" + shop_no + "\"}";
    ws_session->write(message.c_str());
}

void CDlgMain::Order(json_util &util)
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::Order()";
    ::OutputDebugStringA("CDlgMain::Order()");

    try {
        boost::json::value &value = util.get();

        std::string order_seq = strutil::long_to_str(value.at("order_seq").as_int64());
        string regdate = boost::json::value_to<string>(value.at("regdate"));
        string path = path_order + regdate + pos_extra;
        boost::json::value orderList = value.at("pos_order").at("orderList");

        boost::json::object record;
        record["tableNo"] = value.at("table_cd");
        record["orderSeq"] = order_seq;
        record["orderList"] = orderList;
        util.write(path, record, false);

        value.at("status").as_int64() = 1;
        std::string message = util.str();
        ws_session->write(message.c_str());

        string text = "주문 요청입니다 : 테이블 (" + util.get_string("table_cd") + ") 에서 "
            + strutil::long_to_str(orderList.as_array().size()) + "개 주문 [#" + order_seq + "]";
        BOOST_LOG_TRIVIAL(info) << text;
        WriteLog(text);
    }
    catch (std::exception const& e) {
        BOOST_LOG_TRIVIAL(error) << e.what() << endl;
    }
}

void CDlgMain::Menu(json_util& util)
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::Menu()";
    ::OutputDebugStringA("CDlgMain::Menu()");

    try {
        std::string filename = corder_config::get_string("path_status");
        json_util jsonfile;
        jsonfile.load(filename);

        boost::json::value& value = jsonfile.get();

        std::string table_cd = util.get_string("table_cd");
        std::string order_seq = strutil::long_to_str(util.get_int("order_seq"));
        boost::json::value orderList;

        boost::json::array tableList = value.at("tableList").as_array();
        for (auto& table : tableList) {
            if (table.at("tableNo").as_string() == table_cd) {
                orderList = table.at("orderList");
                break;
            }
        }

        boost::json::object object;
        object["msgtype"] = "menu";
        object["shop_no"] = util.get_int("shop_no");
        object["table_cd"] = table_cd;
        object["order_seq"] = order_seq;
        object["orderList"] = orderList;

        std::string message = boost::json::serialize(object);
        ::OutputDebugStringA(message.c_str());
        ws_session->write(message.c_str());
    }
    catch (std::exception const& e) {
        BOOST_LOG_TRIVIAL(error) << e.what() << endl;
    }

}


void CDlgMain::Release()
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::Release()";
    ::OutputDebugStringA("void CDlgMain::Release()");

    if (bRelease == false) {
        bManager = false;

        corder_config::release();
        ioc.stop();
        session_thread.interrupt();
        connect_thread.interrupt();
        session_thread.join();
        connect_thread.join();

        BOOST_LOG_TRIVIAL(info) << "COrder Agent End";
        bRelease = true;
    }
}

void CDlgMain::OnClose()
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::OnClose()";
    ::OutputDebugStringA("void CDlgMain::OnClose()");

    Release();

    CDialogEx::OnClose();
}

void CDlgMain::OnDestroy()
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::OnDestroy()";
    ::OutputDebugStringA("void CDlgMain::OnDestroy()");

    Release();

    CDialogEx::OnDestroy();
}


void CDlgMain::HandleStatus(int status)
{
    BOOST_LOG_TRIVIAL(info) << "CDlgMain::HandleStatus() : " << status;
    ::OutputDebugStringA("CDlgMain::HandleStatus()");

    if ((status == ERROR_CONNECTION_ABORTED) ||
        (status == WSAECONNABORTED) ||
        (status == WSAECONNRESET))
    {
        ioc.stop();
        ws_session.reset();

        //long count = ws_session.use_count();
        //std::string strCount = strutil::long_to_str(count);
        //::OutputDebugStringA(strCount.c_str());

        bConnect = false;

        string text = "서버와의 접속이 끊어졌습니다.";
        BOOST_LOG_TRIVIAL(info) << text;
        WriteLog(text);
    }
    else if (status == WSAECONNREFUSED) {
        ioc.stop();
        ws_session.reset();
        bConnect = false;

        string text = "서버에 접속할 수 없습니다.";
        BOOST_LOG_TRIVIAL(info) << text;
        WriteLog(text);
    }
    else if (status == CONNECTION_SUCCESS) {
        bConnect = true;
        Login();
    }
    else {
        ioc.stop();
        ws_session.reset();
        bConnect = false;

        string text = "서버와의 접속이 끊어졌습니다.";
        BOOST_LOG_TRIVIAL(info) << text;
        WriteLog(text);
    }
}

void CDlgMain::ConnectionManager()
{
    BOOST_LOG_TRIVIAL(debug) << "CDlgMain::ConnectionManager()";
   ::OutputDebugStringA("CDlgMain::ConnectionManager() IN");

    while (bManager) {
        if (bConnect == false) {
            ::OutputDebugStringA("CDlgMain::ConnectionManager() connect");
            string text = "서버에 접속 시도중입니다.";
            BOOST_LOG_TRIVIAL(info) << text;
            WriteLog(text);

            ioc.reset();
            if (ws_session.use_count() == 0) {
                ws_session = std::make_shared<session>(ioc);
                ws_session->set_message_handler(std::bind(&CDlgMain::HandleMessage, this, placeholders::_1));
                ws_session->set_status_hanlder(std::bind(&CDlgMain::HandleStatus, this, placeholders::_1));
                session_thread = boost::thread(boost::bind(&boost::asio::io_service::run, &ioc));
            }
            ioc.post(boost::bind(&service));
        }
        boost::this_thread::sleep(boost::posix_time::millisec(reconnect_time));
        ::OutputDebugStringA("CDlgMain::ConnectionManager() loop");
    }
    ::OutputDebugStringA("CDlgMain::ConnectionManager() OUT");
}

void CDlgMain::HandleMessage(std::string message)
{
    BOOST_LOG_TRIVIAL(debug) << "CDlgMain::HandleMessage() : " << message;

    //std::wstring recv = strutil::mbs_to_wcs(message);
    //::OutputDebugString(recv.c_str());

    try {
        json_util util;
        if (util.parse(message.c_str()) == true) {
            std::string msgtype = util.get_string("msgtype");

            if (msgtype == "login") {
                string text = "로그인에 성공하였습니다.";
                BOOST_LOG_TRIVIAL(info) << text;
                WriteLog(text);
            }
            else if (msgtype == "genpin") {
                string text = "핀 번호가 생성되었습니다 : " + util.get_string("pin");
                BOOST_LOG_TRIVIAL(info) << text;
                thermal.print_pin(print_port.c_str(), util.get_string("pin").c_str(), 3, 3);
                WriteLog(text);
            }
            else if (msgtype == "order") {
                Order(util);
            }
            else if (msgtype == "menu") {
                Menu(util);
            }
            else {
                ::OutputDebugString(L"Unknown Message Type");
            }
        }
        else {
            ::OutputDebugString(L"Parse Error");
        }
    }
    catch (...) {
        ::OutputDebugStringA("HandleMessage exception");
    }
}
