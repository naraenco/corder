#include "stdafx.h"
#include <ShellAPI.h>
#include <functional>
#include <thread>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/filesystem.hpp>
#include "resource.h"
#include "MFCUIDoc.h"
#include "MFCUIView.h"
#include "MainFrm.h"
#include "AfxDialogEx.h"
#include "DlgMain.h"
#include "DlgAppInfo.h"
#include "common/CommonData.h"
#include "bolt/json_util.h"
#include "bolt/strutil.h"
#include "lib/system.h"
#include "main/constant.h"
#include "main/corder_config.h"
#include "main/network.h"

net::io_context ioc;
std::shared_ptr<session> ws_session;
boost::thread session_thread;
boost::thread connect_thread;
//std::thread connect_thread;
corder_config* corder_config::instance_ = nullptr;

static void service()
{
    ::OutputDebugStringA("static void service()");

    auto const host = "127.0.0.1";
    auto const port = "19000";
    auto const text = "";

    try {
        ws_session->start(host, port);
    }
    catch (...) {

    }
}

void run()
{
    ws_session = std::make_shared<session>(ioc);
    session_thread = boost::thread(boost::bind(&boost::asio::io_service::run, &ioc));
    ioc.post(boost::bind(&service));
}


IMPLEMENT_DYNAMIC(CDlgMain, CDialogEx)

CDlgMain::CDlgMain(CWnd* pParent /*=NULL*/)
    : CDialogEx(CDlgMain::IDD, pParent)
{
    m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
    pMFCUIView = (CMFCUIView*)pParent;
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


void CDlgMain::WriteLog(std::wstring value)
{
    logNo++;
    wstring no = cbolt::strutil::long_to_wstr(logNo);
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
    
    //setlocale(LC_ALL, "");

    logNo = 0;
    bManager = true;
    bConnect = false;

    char module[_MAX_PATH] = { 0, };
    GetModuleFileNameA(NULL, module, _MAX_PATH);
    string path = module;
    int pos = (int)path.find_last_of("\\");
    path = path.erase(pos + 1, path.length());
    path += "config.json";

    if (boost::filesystem::exists(path)) {
        corder_config::instance()->get_config()->load(path.c_str());
        ::OutputDebugStringA(corder_config::get_string("db_port").c_str());
    }
    else {
        ::OutputDebugString(L"<config.json> file not found");
    }

    //connect_thread = std::thread(&CDlgMain::ConnectionManager);
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

    commonData.m_icon_sh_start.Load(IDB_PNG_PINGEN, _T("PNG"), AfxGetApp()->m_hInstance);
    CommonData::draw(graphics, commonData.m_icon_sh_start, nServiceX, nServiceY, nServiceSpace, rcIconStart);

    // function
    int nFunctionX = nListX + nListWidth - (32 * 4) - 60;
    int nFunctionY = nListY + nListHeight + 10;
    int nFunctionSpace = 20;

    CommonData::draw(graphics, commonData.m_icon_document, nFunctionX, nFunctionY, nFunctionSpace, rcIconDocument);
    CommonData::draw(graphics, commonData.m_icon_folderdoc, nFunctionX, nFunctionY, nFunctionSpace, rcIconFolderDoc);
    CommonData::draw(graphics, commonData.m_icon_trash, nFunctionX, nFunctionY, nFunctionSpace, rcIconTrash);
    CommonData::draw(graphics, commonData.m_icon_refresh, nFunctionX, nFunctionY, nFunctionSpace, rcRefresh);

    // etc
    commonData.m_image_body.Load(IDB_PNG_TEXT, _T("PNG"), AfxGetApp()->m_hInstance);
    graphics.DrawImage(commonData.m_image_body,
        rcWindow.right - (commonData.m_image_body.m_pBitmap->GetWidth() + 10),
        rcWindow.top + 20,
        commonData.m_image_body.m_pBitmap->GetWidth(), commonData.m_image_body.m_pBitmap->GetHeight());
}

void CDlgMain::OnLButtonDown(UINT nFlags, CPoint point)
{
    if (IsPosition(rcIconStart, point))
    {
        GenPin();
    }
    else if (IsPosition(rcIconDocument, point))
    {
        Connect();
    }
    else if (IsPosition(rcIconFolderDoc, point))
    {
        Login();
    }
    else if (IsPosition(rcIconTrash, point))
    {
        if (m_list_main.GetItemCount() == 0) return;
        if (MessageBox(_T("All lists are deleting.\nWould you like to process?"), _T("Bolt Text Manager"), MB_YESNO) == IDYES)
        {
            m_list_main.DeleteAllItems();
        }
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
    //if (IsPosition(rcIconStart, point))
    //{
    //    SetCursor(LoadCursor(NULL, IDC_HAND));
    //}
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
    //else
    //{
    //    SetCursor(LoadCursor(NULL, IDC_ARROW));
    //}
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
    ::OutputDebugStringA("CDlgMain::Connect()");
}

void CDlgMain::Login()
{
    ::OutputDebugStringA("CDlgMain::Login()");
    auto const text = "{\"msgtype\":\"login\",\"shop_no\": \"3062\",\"auth_key\":\"4285\"}";
    ws_session->write(text);
}

BOOL CDlgMain::GenPin()
{
    ::OutputDebugStringA("CDlgMain::GenPin");
    auto const text = "{\"msgtype\":\"genpin\",\"shop_no\": \"3062\"}";
    ws_session->write(text);

    return TRUE;
}

void CDlgMain::OnClose()
{
    CDialogEx::OnClose();
}

void CDlgMain::OnDestroy()
{
    ::OutputDebugStringA("void CDlgMain::OnDestroy()");
    bManager = false;
    CDialogEx::OnDestroy();
}

void CDlgMain::HandleMessage(std::string message)
{
    ::OutputDebugStringA(message.c_str());
    std::wstring recv = cbolt::strutil::mbs_to_wcs(message);
    WriteLog(recv);

    try {
        json_util util;
        util.parse(message.c_str());
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
    catch (...) {
        ::OutputDebugStringA("process_message exception");
    }
}

void CDlgMain::HandleStatus(int status)
{
    ::OutputDebugStringA("CDlgMain::HandleStatus()");
    if ((status == ERROR_CONNECTION_ABORTED) ||
        (status == WSAECONNABORTED) ||
        (status == WSAECONNRESET))
    {
        bConnect = false;
        ioc.stop();

        long count = ws_session.use_count();
        std::string strCount = cbolt::strutil::long_to_str(count);
        ::OutputDebugStringA(strCount.c_str());

        ws_session.reset();
        count = ws_session.use_count();
        strCount = cbolt::strutil::long_to_str(count);
        ::OutputDebugStringA(strCount.c_str());

        //session_thread.join();
    }
    else if (status == CONNECTION_SUCCESS) {
        bConnect = true;
    }
}

void CDlgMain::ConnectionManager()
{
   ::OutputDebugStringA("CDlgMain::ConnectionManager() in");

    while (bManager) {
        if (bConnect == false) {
            ioc.reset();
            ws_session = std::make_shared<session>(ioc);
            ws_session->set_message_handler(std::bind(&CDlgMain::HandleMessage, this, placeholders::_1));
            ws_session->set_status_hanlder(std::bind(&CDlgMain::HandleStatus, this, placeholders::_1));
            session_thread = boost::thread(boost::bind(&boost::asio::io_service::run, &ioc));
            ioc.post(boost::bind(&service));
            //session_thread.join();
            //ws_session.reset();
            //ioc.stop();
        }
        boost::this_thread::sleep(boost::posix_time::millisec(RECONNECT_TIME));
        ::OutputDebugStringA("CDlgMain::ConnectionManager() loop");
    }

    ::OutputDebugStringA("CDlgMain::ConnectionManager() out");
}
