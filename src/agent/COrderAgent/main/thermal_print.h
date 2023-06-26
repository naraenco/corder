#include <Windows.h>
#include <string>

using namespace std;

class thermal_print
{
public:
    thermal_print();
    ~thermal_print();
    bool print_pin(string comport, string pin, string created_at, int width=3, int height=3);

private:
    void print_com_state(DCB dcb);
};
