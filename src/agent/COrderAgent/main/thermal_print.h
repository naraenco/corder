#include <Windows.h>

using namespace std;

class thermal_print
{
public:
    thermal_print();
    ~thermal_print();
    bool print_pin(const char* comport, const char* pin, int width = 3, int height = 3);

private:
    void print_com_state(DCB dcb);
};
