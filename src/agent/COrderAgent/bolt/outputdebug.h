#pragma once

using namespace std;

namespace cbolt
{

class OutputDebug
{
public:
    void write(const char *fmtstr, ...);
    void write(const wchar_t *fmtstr, ...);

private:
    static const int BUFFERSIZE = 8192;
};

}

extern cbolt::OutputDebug outdebug;
