#pragma once
#include <Windows.h>
#include "ServiceBase.h"

class CService : public CServiceBase
{
protected:
    virtual BOOL InitService()
    {
        //vgs.init();
        return TRUE;
    }

    virtual void ExitService()
    {
        //vgs.stop();
        ::SetEvent(m_hStopEvent);
    }

private:
    //vgs_server vgs;
};
