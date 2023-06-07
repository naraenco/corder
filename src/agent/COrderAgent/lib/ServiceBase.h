#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include <Windows.h>
//#include "vgs_server.h"

struct SERVICEINSTALLSTRUCT
{
    LPCWSTR lpServiceName; // name of service to start
    LPCWSTR lpDisplayName; // display name
    DWORD dwDesiredAccess; // type of access to service
    DWORD dwServiceType; // type of service
    DWORD dwStartType; // when to start service
    DWORD dwErrorControl; // severity of service failure
    LPCWSTR lpBinaryPathName; // name of binary file
    LPCWSTR lpLoadOrderGroup; // name of load ordering group
    LPDWORD lpdwTagId; // tag identifier
    LPCWSTR lpDependencies; // array of dependency names
    LPCWSTR lpServiceStartName; // account name 
    LPCWSTR lpPassword; // account password
};

class CServiceBase
{
public:
    //CServiceBase(LPCWSTR szServiceName, LPCWSTR szDisplayName, LPCWSTR szDescription, LPCWSTR szPath);
    //virtual ~CServiceBase() = 0;
    void init(LPCWSTR szServiceName, LPCWSTR szDisplayName, LPCWSTR szDescription, LPCWSTR szPath);
    virtual void uninit();

    virtual DWORD Main(DWORD dwArgc, LPSTR* lpszArgv);
    void ServiceMain(DWORD dwArgc, LPWSTR* lpszArgv);
    void Handler(DWORD dwOpcode);

    void SetDefaultLogEventID(DWORD dwEventID) { m_dwDefaultLogEventID = dwEventID; }
    BOOL IsInstalled() { return m_bService; }
    virtual BOOL InstallService(DWORD dwStartType);
    virtual BOOL UninstallService();
    virtual void LogEvent(LPCWSTR pFormat, ...);
    void SetServiceStatus(DWORD dwState);


protected:
    virtual BOOL PreInstallService(SERVICEINSTALLSTRUCT& si) { return TRUE; }
    virtual BOOL InitService() { return TRUE; }
    virtual void ExitService() 
    {
        // empty
    }
    virtual void Run();

public:
    HANDLE m_hStopEvent;
    WCHAR m_szServiceName[256];
    WCHAR m_szDisplayName[256];
    WCHAR m_szDescription[256];
    WCHAR m_szFileName[MAX_PATH];
    SERVICE_STATUS_HANDLE m_hServiceStatus;
    SERVICE_STATUS m_status;
    DWORD m_dwThreadID;
    BOOL m_bService;

    DWORD m_dwDefaultLogEventID;
    BOOL CheckService(
        LPCWSTR lpMachineName,      // computer name
        LPCWSTR lpDatabaseName,     // SCM database name
        LPCWSTR lpServiceName,      // service name
        LPCWSTR lpFileName);        // module file name

    static CServiceBase* m_pService;
    static CServiceBase* GetCurrentService();
    static void WINAPI _ServiceMain(DWORD dwArgc, LPWSTR* lpszArgv) { GetCurrentService()->ServiceMain(dwArgc, lpszArgv); }
    static void WINAPI _Handler(DWORD dwOpcode) { GetCurrentService()->Handler(dwOpcode); }
};
