#pragma once


class CCOrderAgentDlg : public CDialogEx
{
public:
    CCOrderAgentDlg(CWnd* pParent = nullptr);
    ~CCOrderAgentDlg();
    void Init();

    // 대화 상자 데이터입니다.
#ifdef AFX_DESIGN_TIME
    enum { IDD = IDD_CORDERAGENT_DIALOG };
#endif

protected:
    virtual void DoDataExchange(CDataExchange* pDX);

protected:
    HICON m_hIcon;

    virtual BOOL OnInitDialog();
    afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
    afx_msg void OnPaint();
    afx_msg HCURSOR OnQueryDragIcon();
    DECLARE_MESSAGE_MAP()

public:
    afx_msg void OnBnClickedButtonGenpin();
    afx_msg void OnBnClickedButtonConnect();
    afx_msg void OnBnClickedButtonLogin();
};
