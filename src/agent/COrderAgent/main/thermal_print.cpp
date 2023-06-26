#include <boost/log/trivial.hpp>
#include <stdio.h>
#include <tchar.h>
#include "thermal_print.h"


thermal_print::thermal_print()
{
}

thermal_print::~thermal_print()
{

}

void thermal_print::print_com_state(DCB dcb)
{
    //  Print some of the DCB structure values
    wprintf(TEXT("\nBaudRate = %d, ByteSize = %d, Parity = %d, StopBits = %d\n"),
        dcb.BaudRate,
        dcb.ByteSize,
        dcb.Parity,
        dcb.StopBits);

    BOOST_LOG_TRIVIAL(info) << "nBaudRate = " << dcb.BaudRate \
        << ", ByteSize = " << dcb.ByteSize \
        << ", Parity = " << dcb.Parity \
        << ", StopBits = " << dcb.StopBits;
}

bool thermal_print::print_pin(string comport, string pin, string created_at, int width,  int height)
{
    DCB dcb = { 0 };
    BOOL fSuccess;

    //  Open a handle to the specified com port.
    HANDLE hCom = CreateFileA(comport.c_str(),
        GENERIC_READ | GENERIC_WRITE,
        0,              //  must be opened with exclusive-access
        NULL,           //  default security attributes
        OPEN_EXISTING,  //  must use OPEN_EXISTING
        0,              //  not overlapped I/O
        NULL);          //  hTemplate must be NULL for comm devices

    if (hCom == INVALID_HANDLE_VALUE)
    {
        BOOST_LOG_TRIVIAL(error) << "CreateFile failed with error : " << GetLastError();
        return 0;
    }

    //  Initialize the DCB structure.
    SecureZeroMemory(&dcb, sizeof(DCB));
    dcb.DCBlength = sizeof(DCB);

    //  Build on the current configuration by first retrieving all current settings.
    fSuccess = GetCommState(hCom, &dcb);

    if (!fSuccess)
    {
        BOOST_LOG_TRIVIAL(error) << "GetCommState failed with error : " << GetLastError();
        return 0;
    }

    //print_com_state(dcb);       //  Output to console

    //  Fill in some DCB values and set the com state: 
    //  57,600 bps, 8 data bits, no parity, and 1 stop bit.
    dcb.BaudRate = CBR_9600;        //  baud rate
    dcb.ByteSize = 8;               //  data size, xmit and rcv
    dcb.Parity = NOPARITY;          //  parity bit
    dcb.StopBits = ONESTOPBIT;      //  stop bit

    fSuccess = SetCommState(hCom, &dcb);

    if (!fSuccess)
    {
        BOOST_LOG_TRIVIAL(error) << "SetCommState failed with error : " << GetLastError();
        return false;
    }

    //  Get the comm config again.
    fSuccess = GetCommState(hCom, &dcb);

    if (!fSuccess)
    {
        BOOST_LOG_TRIVIAL(error) << "GetCommState failed with error : " << GetLastError();
        return false;
    }

    //print_com_state(dcb);

    BOOST_LOG_TRIVIAL(info) << "Serial port %s successfully reconfigured : " << comport;

    DWORD bytesWritten;
    char command[20];

    memset(command, 0, 20);
    sprintf_s(command, sizeof(command), "\x1B!%c", ((width - 1) << 4) | (height - 1));
    if (!WriteFile(hCom, command, strlen(command), &bytesWritten, NULL)) {
        BOOST_LOG_TRIVIAL(error) << "font size change error";
        return false;
    }

    //char data[] = "PIN : 3062\n\n\n\n\n\n\n\n\n\n";
    string data = created_at + "\nPIN : " + pin + "\n\n\n\n\n\n\n\n\n\n";
    if (!WriteFile(hCom, data.c_str(), data.length(), &bytesWritten, NULL)) {
        BOOST_LOG_TRIVIAL(error) << "pin print error : " << GetLastError();
        CloseHandle(hCom);
        return false;
    }

    memset(command, 0, 20);
    const BYTE cut_command[] = { 0x1B, 0x69 };  // 커팅 명령 (ESC i)
    if (!WriteFile(hCom, cut_command, sizeof(cut_command), &bytesWritten, NULL)) {
        BOOST_LOG_TRIVIAL(error) << "cutting error : " << GetLastError();
        return false;
    }

    return 1;
}
