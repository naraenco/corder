#pragma once
#include "../bolt/json_file.h"

class corder_config
{
private:
    void init() { config = new cbolt::json_file(); }
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

    static wstring get_wstring(const char *key) {
        return instance()->get_config()->get_wstring(key);
    }

    static int get_int(const char *key) {
        return instance()->get_config()->get_int(key);
    }
    static bool get_bool(const char *key) {
        return instance()->get_config()->get_bool(key);
    }

    static cbolt::json_file* get() {
        return instance()->get_config();
    }

    cbolt::json_file* get_config() {
        return config;
    }

private:
    static corder_config* instance_;
    cbolt::json_file* config;

};
