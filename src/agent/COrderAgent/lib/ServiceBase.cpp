#include "ServiceBase.h"
#include <stdio.h>
//#include <thread>
//#include "vgs_server.h"
//#include "vgs_sharedata.h"

CServiceBase* CServiceBase::m_pService = NULL;
CServiceBase* CServiceBase::GetCurrentService()
{
    return m_pService;
}

BOOL CServiceBase::CheckService(LPCWSTR lpMachineName,
    LPCWSTR lpDatabaseName,
    LPCWSTR lpServiceName,
    LPCWSTR lpFileName) 
{
    BOOL bResult = FALSE;
    SC_HANDLE hSCM = 0;
    SC_HANDLE hService = 0;

    hSCM = ::OpenSCManager(lpMachineName, lpDatabaseName, SC_MANAGER_ALL_ACCESS);
    if (hSCM != NULL)
    {
        hService = ::OpenService(hSCM, lpServiceName, SERVICE_QUERY_CONFIG);
        if (hService != NULL)
        {
            if (!lpFileName)
            {
                bResult = TRUE;
            }
            else
            {
                const int bufsize = sizeof(QUERY_SERVICE_CONFIG) + 1024;
                char buf[bufsize];
                QUERY_SERVICE_CONFIG* qsc = (QUERY_SERVICE_CONFIG*)buf;
                DWORD dwNeed = 0;
                BOOL bRet = ::QueryServiceConfig(hService, qsc, bufsize, &dwNeed);
                if (bRet && !lstrcmpi(qsc->lpBinaryPathName, lpFileName)) bResult = TRUE;
            }
            ::CloseServiceHandle(hService);
        }
        ::CloseServiceHandle(hSCM);
    }
    return bResult;
}

//CServiceBase::CServiceBase(LPCWSTR szServiceName, LPCWSTR szDisplayName, LPCWSTR szDescription, LPCWSTR szPath)
void CServiceBase::init(LPCWSTR szServiceName, LPCWSTR szDisplayName, LPCWSTR szDescription, LPCWSTR szPath)
{
    m_pService = this;

    wcscpy(m_szServiceName, szServiceName);
    if (!szDisplayName)
        szDisplayName = szServiceName;
    wcscpy(m_szDisplayName, szDisplayName);
    wcscpy(m_szDescription, szDescription);
    wcscpy(m_szFileName, szPath);

    m_hStopEvent = ::CreateEvent(NULL, TRUE, FALSE, NULL);
    m_bService = FALSE;
    m_hServiceStatus = NULL;
    m_status.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    m_status.dwCurrentState = SERVICE_STOPPED;
    m_status.dwControlsAccepted = SERVICE_ACCEPT_STOP;
    m_status.dwWin32ExitCode = NO_ERROR;
    m_status.dwServiceSpecificExitCode = NO_ERROR;
    m_status.dwCheckPoint = 0;
    m_status.dwWaitHint = 0;
    m_dwDefaultLogEventID = 0;
    m_bService = CheckService(NULL, NULL, m_szServiceName, m_szFileName);
}

// CServiceBase::~CServiceBase()
void CServiceBase::uninit()
{
    ::CloseHandle(m_hStopEvent);
    m_hStopEvent = 0;
    if (m_pService == this) m_pService = NULL;
}

void CServiceBase::ServiceMain(DWORD dwArgc, LPWSTR* lpszArgv)
{
    m_status.dwCurrentState = SERVICE_START_PENDING;
    m_hServiceStatus = RegisterServiceCtrlHandler(m_szServiceName, _Handler);
    if (m_hServiceStatus == NULL)
    {
        LogEvent(L"Handler not installed\n");
        return;
    }
    SetServiceStatus(SERVICE_START_PENDING);
    m_status.dwWin32ExitCode = S_OK;
    m_status.dwCheckPoint = 0;
    m_status.dwWaitHint = 0;

    Run();
}

void CServiceBase::Handler(DWORD dwOpcode)
{
    switch (dwOpcode)
    {
    case SERVICE_CONTROL_STOP:
        SetServiceStatus(SERVICE_STOP_PENDING);
        ExitService();
        break;

    case SERVICE_CONTROL_PAUSE:
        break;

    case SERVICE_CONTROL_CONTINUE:
        break;

    case SERVICE_CONTROL_INTERROGATE:
        break;

    case SERVICE_CONTROL_SHUTDOWN:
        break;

    default:
        LogEvent(L"Bad service request\n");
    }
}

void CServiceBase::Run()
{
    m_dwThreadID = ::GetCurrentThreadId();
    /*
    if (InitService())
    {
        std::thread failover_thread;
        std::thread service_thread(vgs_server::service);
        if (vgs_sharedata::instance()->failover_)
            failover_thread = std::thread(vgs_server::failover);
        std::thread check_thread(vgs_server::check);

        if (m_bService)
        {
            LogEvent(L"Service started\n");
            SetServiceStatus(SERVICE_RUNNING);
        }
        else
        {
            //LogEvent(L"Program started\n");
        }
        ::WaitForSingleObject(m_hStopEvent, INFINITE);
        check_thread.join();
        if (vgs_sharedata::instance()->failover_)
        {
            failover_thread.join();
        }
        service_thread.join();
    }
    ExitService();
    if (m_bService)
    {
        SetServiceStatus(SERVICE_STOPPED);
        LogEvent(L"Service stopped\n");
    }
    else
    {
        LogEvent(L"Program stopped\n");
    }
    vgs_sharedata::release();
    */
}

BOOL CServiceBase::InstallService(DWORD dwStartType)
{
    if (m_bService) return TRUE;

    BOOL bResult = FALSE;

    SC_HANDLE hSCM = ::OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
    if (hSCM == NULL)
    {
        return FALSE;
    }

    SERVICEINSTALLSTRUCT si;
    si.lpServiceName = m_szServiceName;
    si.lpDisplayName = m_szDisplayName;
    si.dwDesiredAccess = SERVICE_ALL_ACCESS;            // type of access to service
    si.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    si.dwStartType = dwStartType;                       // when to start service
    si.dwErrorControl = SERVICE_ERROR_NORMAL;           // severity of service failure
    si.lpBinaryPathName = m_szFileName;                 // name of binary file
    si.lpLoadOrderGroup = NULL;                         // name of load ordering group
    si.lpdwTagId = NULL;                                // tag identifier
    si.lpDependencies = L"\0";                          // array of dependency names
    si.lpServiceStartName = NULL;                       // account name 
    si.lpPassword = NULL;                               // account password

    if (PreInstallService(si))
    {
        SC_HANDLE hService = ::CreateService(
            hSCM,
            si.lpServiceName,
            si.lpDisplayName,
            si.dwDesiredAccess,
            si.dwServiceType,
            si.dwStartType,
            si.dwErrorControl,
            si.lpBinaryPathName,
            si.lpLoadOrderGroup,
            si.lpdwTagId,
            si.lpDependencies,
            si.lpServiceStartName,
            si.lpPassword);

        if (hService)
        {
            SERVICE_DESCRIPTION description = { m_szDescription };
            ChangeServiceConfig2(hService, SERVICE_CONFIG_DESCRIPTION, &description);

            ::CloseServiceHandle(hService);
            bResult = TRUE;
        }
    }
    ::CloseServiceHandle(hSCM);

    if (bResult) {
        printf("service installed\n");
    }
    
    return bResult;
}

BOOL CServiceBase::UninstallService()
{
    if (!m_bService) return TRUE;

    SC_HANDLE hSCM = ::OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
    if (hSCM == NULL)
    {
        return FALSE;
    }

    SC_HANDLE hService = ::OpenService(hSCM, m_szServiceName, SERVICE_STOP | DELETE);
    if (hService == NULL)
    {
        ::CloseServiceHandle(hSCM);
        return FALSE;
    }
    SERVICE_STATUS status;
    ::ControlService(hService, SERVICE_CONTROL_STOP, &status);

    BOOL bDelete = ::DeleteService(hService);
    ::CloseServiceHandle(hService);
    ::CloseServiceHandle(hSCM);

    if (!bDelete) {
        DWORD err = ::GetLastError();
        printf("service delete error : %lu", err);
        return FALSE;
    }

    printf("service deleted\n");

    return TRUE;
}

void CServiceBase::LogEvent(LPCWSTR pFormat, ...)
{
    WCHAR   chMsg[256];
    HANDLE  hEventSource;
    LPWSTR  lpszStrings[1];
    va_list pArg;

    va_start(pArg, pFormat);
    _vsnwprintf_s(chMsg, 256, pFormat, pArg);
    va_end(pArg);

    lpszStrings[0] = chMsg;

    if (m_bService)
    {
        hEventSource = RegisterEventSource(NULL, m_szServiceName);
        if (hEventSource != NULL)
        {
            ::ReportEvent(
                hEventSource,
                EVENTLOG_INFORMATION_TYPE,
                0,
                m_dwDefaultLogEventID, // EventID
                NULL,
                1,
                0,
                (LPCWSTR*)&lpszStrings[0],
                NULL
            );
            DeregisterEventSource(hEventSource);
        }
    }
    if (!m_bService)
    {
        fwprintf(stderr, L"%s", chMsg);
    }
#ifdef _DEBUG
    OutputDebugString(chMsg);
#endif
}

void CServiceBase::SetServiceStatus(DWORD dwState)
{
    m_status.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    m_status.dwWin32ExitCode = NO_ERROR;
    m_status.dwServiceSpecificExitCode = NO_ERROR;
    
    switch (dwState) {
    case SERVICE_START_PENDING:
        m_status.dwControlsAccepted = 0;
        m_status.dwCheckPoint = 1;
        m_status.dwWaitHint = 1000;
        m_status.dwCurrentState = SERVICE_START_PENDING;
        break;
    case SERVICE_RUNNING:
        m_status.dwControlsAccepted = SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN;
        m_status.dwCheckPoint = 0;
        m_status.dwCurrentState = SERVICE_RUNNING;
        break;
    case SERVICE_STOP_PENDING:
        m_status.dwControlsAccepted = 0;
        m_status.dwCheckPoint = 1;
        m_status.dwWaitHint = 1000;
        m_status.dwCurrentState = SERVICE_STOP_PENDING;
        break;
    case SERVICE_STOPPED:
        m_status.dwControlsAccepted = 0;
        m_status.dwCheckPoint = 0;
        m_status.dwCurrentState = SERVICE_STOPPED;
        break;
    default:
        m_status.dwCurrentState = dwState;
        break;
    }
    ::SetServiceStatus(m_hServiceStatus, &m_status);
}

DWORD CServiceBase::Main(DWORD dwArgc, LPSTR* lpArgs)
{
    if (dwArgc > 1)
    {
        char* szToken = lpArgs[1];
         
        if (!_strcmpi(szToken, "/install"))
        {
            InstallService(SERVICE_DEMAND_START);
            return 0;
        }
        if (!_strcmpi(szToken, "/auto"))
        {
            InstallService(SERVICE_AUTO_START);
            return 0;
        }
        if (!_strcmpi(szToken, "/uninstall"))
        {
            UninstallService();
            return 0;
        }
    }

    if (!m_bService)
    {
        Run();
    }
    else
    {
        SERVICE_TABLE_ENTRYW ste[] =
        {
            { m_szServiceName, (LPSERVICE_MAIN_FUNCTION)_ServiceMain },
            { NULL, NULL }
        };
        if (!::StartServiceCtrlDispatcherW(ste))
        {
            m_status.dwWin32ExitCode = ::GetLastError();
            printf("StartServiceCtrlDispatcher error : %lu", m_status.dwWin32ExitCode);
        }
    }
    return m_status.dwWin32ExitCode;
}
