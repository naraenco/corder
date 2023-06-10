#include "outputdebug.h"
#include <windows.h>
#include <iostream>

cbolt::OutputDebug outdebug;

namespace cbolt
{

void OutputDebug::write(const char *fmtstr, ...)
{
    char buffer[BUFFERSIZE] = { 0, };
    va_list vl;
    va_start(vl, fmtstr);
    _vsnprintf_s(buffer, BUFFERSIZE, fmtstr, vl);
    va_end(vl);
    OutputDebugStringA(buffer);
    std::cout << buffer << std::endl;
}

void OutputDebug::write(const wchar_t *fmtstr, ...)
{
    wchar_t widebuff[BUFFERSIZE] = { 0, };
    va_list vl;
    va_start(vl, fmtstr);
    _vsnwprintf_s(widebuff, BUFFERSIZE, fmtstr, vl);
    va_end(vl);
    OutputDebugStringW(widebuff);
    std::wcout << widebuff << std::endl;
}

}