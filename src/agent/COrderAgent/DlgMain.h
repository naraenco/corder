#pragma once
#pragma warning(disable:4244)
#include <string>
#include "bolt/json_util.h"
#include "main/thermal_print.h"


class CMFCUIView;
class CDlgMain : public CDialogEx
{
    DECLARE_DYNAMIC(CDlgMain)

public:
    CDlgMain(CWnd* pParent = NULL);
    virtual ~CDlgMain();
    BOOL Create(CRect& rcWnd, UINT nIDTemplate, CWnd* pParentWnd);
    enum { IDD = IDD_DLG_MAIN };

public:
    void Resize(CRect& rect);
    void ComponentResize();
    void WriteLog(std::string value);
    void WriteLog(std::wstring value);

    void About();
    void Exit();
    void Config();

    void Release();
    void Connect();
    void Login();
    void GenPin();
    void Order(json_util& util);
    void Menu(json_util& util);

    void HandleMessage(std::string message);
    void HandleStatus(int status);
    void ConnectionManager();

private:
    bool bManager;
    bool bConnect;
    bool bRelease;
    long log_no;
    long reconnect_time;

    thermal_print thermal;
    std::string print_port;
    int print_font_width;
    int print_font_height;
    std::string pos_extra;
    std::string path_order;
    std::string shop_no;
    std::string auth_key;

    CMFCUIView* pMFCUIView;
    CListCtrl m_list_main;
    CRect m_WindowRect;
    CImageList* m_pDragImage;
    HICON m_hIcon;
    UINT nListWidth;
    UINT nListHeight;
    UINT nListX;
    UINT nListY;
    CRect rcIconGenPin;
    CRect rcIconDocument;
    CRect rcIconFolderDoc;
    CRect rcIconTrash;
    CRect rcRefresh;

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    DECLARE_MESSAGE_MAP()

public:
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
    afx_msg HCURSOR OnQueryDragIcon();
    afx_msg void OnPaint();
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg void OnClose();
    afx_msg void OnDestroy();
};

