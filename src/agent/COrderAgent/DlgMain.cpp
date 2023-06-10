#include "stdafx.h"
#include <ShellAPI.h>
#include <locale>
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
#include "main/network.h"
#include "main/corder_config.h"

net::io_context ioc;
std::shared_ptr<session> ws_session;
boost::thread_group io_threads;
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
    ioc.post(boost::bind(&service));
    io_threads.create_thread(boost::bind(&boost::asio::io_service::run, &ioc));
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

BOOL CDlgMain::OnInitDialog()
{
    CDialogEx::OnInitDialog();

    MoveWindow(m_WindowRect.left, m_WindowRect.top, m_WindowRect.right - m_WindowRect.left, m_WindowRect.bottom - m_WindowRect.top, TRUE);

    m_list_main.InsertColumn(0, _T("No"), LVCFMT_LEFT, 0);
    m_list_main.InsertColumn(1, _T("DateTime"), LVCFMT_LEFT, 150);
    m_list_main.InsertColumn(2, _T("Log"), LVCFMT_LEFT, 350);
    m_list_main.ShowWindow(SW_SHOW);

    ComponentResize();
    
    setlocale(LC_ALL, "");


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
    if (IsPosition(rcIconStart, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
    else if (IsPosition(rcIconDocument, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
    else if (IsPosition(rcIconFolderDoc, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
    else if (IsPosition(rcIconTrash, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
    else if (IsPosition(rcRefresh, point))
    {
        SetCursor(LoadCursor(NULL, IDC_HAND));
    }
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
    pMFCUIView->Exit();
}

BOOL CDlgMain::GenPin()
{
    ::OutputDebugStringA("CDlgMain::GenPin");
    auto const text = "{\"msgtype\":\"genpin\",\"shop_no\": \"3062\"}";
    ws_session->write(text);

    return TRUE;
}

void CDlgMain::Connect()
{
    ::OutputDebugStringA("CDlgMain::Connect()");
    run();
}

void CDlgMain::Login()
{
    ::OutputDebugStringA("CDlgMain::Login()");
    auto const text = "{\"msgtype\":\"login\",\"shop_no\": \"3062\",\"auth_key\":\"4285\"}";
    ws_session->write(text);
}

void CDlgMain::OnClose()
{
    CDialogEx::OnClose();
}

void CDlgMain::OnDestroy()
{
    CDialogEx::OnDestroy();
}

/*
    m_list_main.InsertItem(0, no);
    m_list_main.SetItemText(0, 1, datetime);
    m_list_main.SetItemText(0, 2, text);
*/
