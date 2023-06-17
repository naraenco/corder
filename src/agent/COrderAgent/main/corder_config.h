#pragma once
#include "../bolt/json_util.h"

class corder_config
{
private:
    void init() { config = new json_util(); }
    void uninit() { delete config; }

public:

    static corder_config* instance() {
        if (instance_ == nullptr)
        {
            instance_ = new corder_config();
            instance_->init();
        }
        return instance_;
    }
    static void release() {
        if (instance_ != nullptr) {
            instance_->uninit();
            delete instance_;
            instance_ = nullptr;
        }
    }
    static string get_string(const char *key) {
        return instance()->get_config()->get_string(key);
    }

    static int get_int(const char *key) {
        return instance()->get_config()->get_int(key);
    }
    static bool get_bool(const char *key) {
        return instance()->get_config()->get_bool(key);
    }

    static json_util* get() {
        return instance()->get_config();
    }

    json_util* get_config() {
        return config;
    }

private:
    static corder_config* instance_;
    json_util* config;
};
