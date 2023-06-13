﻿#pragma once
#include <string>


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

    void Connect();
    void Login();
    void GenPin();

    void HandleMessage(std::string message);
    void HandleStatus(int status);
    void ConnectionManager();
    
private:
    CMFCUIView* pMFCUIView;
    bool bManager;
    bool bConnect;

    CListCtrl m_list_main;
    CRect m_WindowRect;
    CImageList* m_pDragImage;

    HICON m_hIcon;

    UINT nListWidth;
    UINT nListHeight;
    UINT nListX;
    UINT nListY;

    long logNo;

    CRect rcIconStart;
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
