#include <Windows.h>
#include <string>

class thermal_print
{
public:
    thermal_print();
    ~thermal_print();
    bool print_pin(std::string seport,
        std::string pin,
        std::string created_at,
        int width=3, 
        int height=3);

private:
    void print_com_state(DCB dcb);
};
