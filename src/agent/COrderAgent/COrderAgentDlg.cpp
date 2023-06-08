#include "pch.h"
#include "framework.h"
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/filesystem.hpp>
#include "afxdialogex.h"
#include "COrderAgent.h"
#include "COrderAgentDlg.h"
#include "main/network.h"
#include "corder_config.h"


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


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


class CAboutDlg : public CDialogEx
{
public:
    CAboutDlg();

#ifdef AFX_DESIGN_TIME
    enum { IDD = IDD_ABOUTBOX };
#endif

protected:
    virtual void DoDataExchange(CDataExchange* pDX);

protected:
    DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(IDD_ABOUTBOX)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


CCOrderAgentDlg::CCOrderAgentDlg(CWnd* pParent /*=nullptr*/)
    : CDialogEx(IDD_CORDERAGENT_DIALOG, pParent)
{
    m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

CCOrderAgentDlg::~CCOrderAgentDlg()
{
    ioc.stop();
    io_threads.join_all();
    ioc.reset();
    corder_config::release();
}

void CCOrderAgentDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CCOrderAgentDlg, CDialogEx)
    ON_WM_SYSCOMMAND()
    ON_WM_PAINT()
    ON_WM_QUERYDRAGICON()
    ON_BN_CLICKED(IDC_BUTTON_GENPIN, &CCOrderAgentDlg::OnBnClickedButtonGenpin)
    ON_BN_CLICKED(IDC_BUTTON_CONNECT, &CCOrderAgentDlg::OnBnClickedButtonConnect)
    ON_BN_CLICKED(IDC_BUTTON_LOGIN, &CCOrderAgentDlg::OnBnClickedButtonLogin)
END_MESSAGE_MAP()


BOOL CCOrderAgentDlg::OnInitDialog()
{
    CDialogEx::OnInitDialog();

    // 시스템 메뉴에 "정보..." 메뉴 항목을 추가합니다.
    // IDM_ABOUTBOX는 시스템 명령 범위에 있어야 합니다.
    ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
    ASSERT(IDM_ABOUTBOX < 0xF000);

    CMenu* pSysMenu = GetSystemMenu(FALSE);
    if (pSysMenu != nullptr)
    {
        BOOL bNameValid;
        CString strAboutMenu;
        bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
        ASSERT(bNameValid);
        if (!strAboutMenu.IsEmpty())
        {
            pSysMenu->AppendMenu(MF_SEPARATOR);
            pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
        }
    }

    SetIcon(m_hIcon, TRUE);
    SetIcon(m_hIcon, FALSE);

    // TODO: 여기에 추가 초기화 작업을 추가합니다.
    Init();

    return TRUE;
}

void CCOrderAgentDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
    if ((nID & 0xFFF0) == IDM_ABOUTBOX)
    {
        CAboutDlg dlgAbout;
        dlgAbout.DoModal();
    }
    else
    {
        CDialogEx::OnSysCommand(nID, lParam);
    }
}

void CCOrderAgentDlg::OnPaint()
{
    if (IsIconic())
    {
        CPaintDC dc(this);

        SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

        int cxIcon = GetSystemMetrics(SM_CXICON);
        int cyIcon = GetSystemMetrics(SM_CYICON);
        CRect rect;
        GetClientRect(&rect);
        int x = (rect.Width() - cxIcon + 1) / 2;
        int y = (rect.Height() - cyIcon + 1) / 2;

        dc.DrawIcon(x, y, m_hIcon);
    }
    else
    {
        CDialogEx::OnPaint();
    }
}

HCURSOR CCOrderAgentDlg::OnQueryDragIcon()
{
    return static_cast<HCURSOR>(m_hIcon);
}

void CCOrderAgentDlg::Init()
{

    char module[_MAX_PATH] = { 0, };
    GetModuleFileNameA(NULL, module, _MAX_PATH);
    string path = module;
    int pos = (int)path.find_last_of("\\");
    path = path.erase(pos + 1, path.length());
    path += "config.json";

    if (!boost::filesystem::exists(path))
    {
        cout << "<config.json> file not found" << endl;
        return;
    }

    corder_config::instance()->get_config()->load(path.c_str());
    ::OutputDebugStringA(corder_config::get_string("db_port").c_str());
}

void CCOrderAgentDlg::OnBnClickedButtonConnect()
{
    ::OutputDebugStringA("OnBnClickedButtonConnect");

    run();
}

void CCOrderAgentDlg::OnBnClickedButtonLogin()
{
    ::OutputDebugStringA("OnBnClickedButtonLogin");
    auto const text = "{\"msgtype\":\"login\",\"shop_no\": \"3062\",\"auth_key\":\"4285\"}";
    ws_session->write(text);
}

void CCOrderAgentDlg::OnBnClickedButtonGenpin()
{
    ::OutputDebugStringA("OnBnClickedButtonGenpin");
    auto const text = "{\"msgtype\":\"genpin\",\"shop_no\": \"3062\"}";
    ws_session->write(text);
}
