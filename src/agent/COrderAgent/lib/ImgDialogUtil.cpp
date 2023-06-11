#include "StdAfx.h"
#include "./ImgDialogUtil.h"

CString CImgDialogUtil::GetModulePath(HMODULE hModule /* = NULL */)
{
    TCHAR buf[MAX_PATH] = { '\0' };
    CString strDir, strTemp;

    ::GetModuleFileName(hModule, buf, MAX_PATH);
    strTemp = buf;
    strDir = strTemp.Left(strTemp.ReverseFind('\\') + 1);
    return strDir;
}

BOOL CImgDialogUtil::IsFileExist(LPCTSTR lpszFilePath)
{
    BOOL bExist = FALSE;
    HANDLE hFile = NULL;
    hFile = CreateFile(lpszFilePath
        , GENERIC_READ
        , FILE_SHARE_READ | FILE_SHARE_WRITE
        , NULL
        , OPEN_EXISTING
        , 0
        , 0
    );

    if (hFile != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hFile);
        bExist = TRUE;
    }
    return bExist;
}

BOOL CImgDialogUtil::ExtractResourceToFile(LPCTSTR lpszType
    , UINT nResID
    , LPCTSTR lpszFilename
    , HMODULE hModule
)
{
    HRSRC hRes = ::FindResource(hModule, MAKEINTRESOURCE(nResID), lpszType);
    if (hRes == NULL)
    {
        ATLASSERT(FALSE);
        return FALSE;
    }

    DWORD dwSize = ::SizeofResource(hModule, hRes);
    if (dwSize == 0)
    {
        ATLASSERT(FALSE);
        return FALSE;
    }

    HGLOBAL hGlobal = ::LoadResource(hModule, hRes);
    if (hGlobal == NULL)
    {
        ATLASSERT(FALSE);
        return FALSE;
    }

    LPVOID pBuffer = ::LockResource(hGlobal);
    if (pBuffer == NULL)
    {
        ATLASSERT(FALSE);
        ::FreeResource(hGlobal);
        return FALSE;
    }

    HANDLE hFile = ::CreateFile(lpszFilename
        , GENERIC_WRITE
        , FILE_SHARE_WRITE | FILE_SHARE_READ
        , NULL
        , CREATE_ALWAYS
        , 0
        , NULL
    );
    if (hFile == NULL)
    {
        ATLASSERT(FALSE);
        ::FreeResource(hGlobal);
        return FALSE;
    }

    DWORD dwWritten = 0;
    ::WriteFile(hFile, pBuffer, dwSize, &dwWritten, NULL);
    if (dwWritten != dwSize)
    {
        ATLASSERT(FALSE);
        ::FreeResource(hGlobal);
        return FALSE;
    }

    ::FlushFileBuffers(hFile);
    ::CloseHandle(hFile);
    ::FreeResource(hGlobal);

    return TRUE;
}// ExtractResourceToFile

Image* CImgDialogUtil::LoadImage(UINT nID, LPCTSTR lpszType, HINSTANCE hInstance /*=NULL*/)
{
    Image* pImage = NULL;

    if (lpszType == RT_BITMAP)
    {
        HBITMAP hBitmap = ::LoadBitmap(hInstance, MAKEINTRESOURCE(nID));
        pImage = (Image*)Bitmap::FromHBITMAP(hBitmap, 0);
        ::DeleteObject(hBitmap);
        return pImage;
    }

    hInstance = (hInstance == NULL) ? ::AfxGetResourceHandle() : hInstance;
    HRSRC hRsrc = ::FindResource(hInstance, MAKEINTRESOURCE(nID), lpszType);
    ASSERT(hRsrc != NULL);

    DWORD dwSize = ::SizeofResource(hInstance, hRsrc);
    LPBYTE lpRsrc = (LPBYTE)::LoadResource(hInstance, hRsrc);
    ASSERT(lpRsrc != NULL);

    HGLOBAL hMem = ::GlobalAlloc(GMEM_FIXED, dwSize);
    LPBYTE pMem = (LPBYTE)::GlobalLock(hMem);
    memcpy(pMem, lpRsrc, dwSize);
    IStream* pStream = NULL;
    ::CreateStreamOnHGlobal(hMem, FALSE, &pStream);

    pImage = Image::FromStream(pStream);

    ::GlobalUnlock(hMem);
    pStream->Release();
    ::FreeResource(lpRsrc);

    return pImage;
}
