CREATE TABLE `member` (
    `member_srl`                bigint NOT NULL PRIMARY KEY,
    `user_id`                   varchar(80) NOT NULL,
    `password`                  varchar(250) NOT NULL,
    `email_address`             varchar(80) NOT NULL,
    `email_id`                  varchar(80) NOT NULL,
    `email_host`                varchar(80) NULL,
    `phone_number`              varchar(80) NULL,
    `phone_country`             varchar(10) NULL,
    `phone_type`                varchar(10) NULL,
    `user_name`                 varchar(40) NOT NULL,
    `nick_name`                 varchar(40) NOT NULL,
    `find_account_question`     bigint NULL,
    `find_account_answer`       varchar(250) NULL,
    `homepage`                  varchar(250) NULL,
    `blog`                      varchar(250) NULL,
    `birthday`                  char(8) NULL,
    `allow_mailing`             char(1) NOT NULL DEFAULT 'Y',
    `allow_message`             char(1) NOT NULL DEFAULT 'Y',
    `denied`                    char(1) NULL DEFAULT 'N',
    `regdate`                   char(14) NULL,
    `ipaddress`                 varchar(60) NULL,
    `last_login`                char(14) NULL,
    `last_login_ipaddress`      varchar(60) NULL,
    `limit_date`                char(14) NULL,
    `change_password_date`      char(14) NULL,
    `is_admin`                  char(1) NULL DEFAULT 'N',
    `description`               text NULL,
    `extra_vars`                text NULL,
    `list_order`                bigint NOT NULL
);

CREATE TABLE `member_group` (
    `group_srl`                 bigint NOT NULL PRIMARY KEY,
    `site_srl`                  bigint NOT NULL DEFAULT 0,
    `list_order`                bigint NOT NULL,
    `title`                     varchar(80) NOT NULL,
    `regdate`                   varchar(14) NULL,
    `is_default`                char(1) NULL DEFAULT 'N',
    `is_admin`                  char(1) NULL DEFAULT 'N',
    `image_mark`                text NULL,
    `description`               text NULL
);

CREATE TABLE `member_group_member` (
    `group_srl`                 bigint NOT NULL,
    `member_srl`                bigint NOT NULL,
    `site_srl`                  bigint NOT NULL DEFAULT 0,
    `regdate`                   char(14) NULL
);

CREATE TABLE `shop` (
    `shop_no`                   bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `shop_cd`                   varchar(20) NOT NULL,
    `name`                      varchar(80) NOT NULL,
    `business_number`           char(10) NULL,
    `phone`                     varchar(20) NOT NULL,
    `postcode`                  varchar(10) NOT NULL,
    `road_address`              varchar(100) NULL,
    `jibun_address`             varchar(100) NULL,
    `extra_address`             varchar(100) NULL,
    `detail_address`            varchar(100) NULL,
    `map_x`                     varchar(50) NOT NULL,
    `map_y`                     varchar(50) NOT NULL,
    `regdate`                   varchar(14) NOT NULL,
    `pos_type`                  varchar(10) NOT NULL COMMENT '"kispay":키스페이'
);

CREATE TABLE `shop_manager` (
    `shop_no`                   bigint NOT NULL,
    `member_srl`                bigint NOT NULL
);

CREATE TABLE `shop_menu` (
    `menu_no`                   bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `name`                      varchar(100) NOT NULL,
    `description`               varchar(250) NULL,
    `price`                     int NOT NULL,
    `discount`                  int NULL,
    `sequence`                  int NOT NULL,
    `visible`                   char(1) NOT NULL DEFAULT 'Y' COMMENT '"Y":노출, "N":감추기',
    `pos_menu`                  text NULL
);

CREATE TABLE `shop_menu_category` (
    `category_no`               bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `category_name`             varchar(100) NOT NULL,
    `step`                      int NULL COMMENT '1:대분류, 2:중분류, 3:소분류',
    `sequence`                  int NOT NULL,
    `visible`                   char(1) NOT NULL DEFAULT 'Y' COMMENT '"Y":노출, "N":감추기'
);

CREATE TABLE `shop_category_menu` (
    `category_no`               bigint NOT NULL,
    `menu_no`                   bigint NOT NULL
);

CREATE TABLE `shop_table` (
    `table_cd`                  varchar(20) NOT NULL PRIMARY KEY,
    `shop_no`                   bigint NOT NULL,
    `name`                      varchar(20) NOT NULL,
    `status`                    char(1) NOT NULL COMMENT '"R":준비, "U":사용중, "N":사용안함',
    `pos_table`                 text NULL
);

CREATE TABLE `shop_order` (
    `order_no`                  bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `shop_no`                   bigint NOT NULL,
    `table_cd`                  varchar(20) NOT NULL,
    `otp_pin`                   char(4) NOT NULL COMMENT '4자리숫자',
    `regdate`                   varchar(14) NOT NULL,
    `status`                    int NOT NULL DEFAULT 0 COMMENT '0:주문 요청, 1:주문 성공',
    `pos_order`                 text NULL
);

CREATE TABLE `files` (
    `file_srl`                  bigint NOT NULL PRIMARY KEY,
    `upload_target_srl`         bigint NOT NULL DEFAULT 0,
    `upload_target_type`        char(3) NULL,
    `sid`                       varchar(60) NULL,
    `module_srl`                bigint NOT NULL DEFAULT 0,
    `download_count`            bigint NOT NULL DEFAULT 0,
    `direct_download`           char(1) NOT NULL DEFAULT 'N',
    `source_filename`           varchar(250) NULL,
    `uploaded_filename`         varchar(250) NULL,
    `file_size`                 bigint NOT NULL DEFAULT 0,
    `comment`                   varchar(250) NULL,
    `isvalid`                   char(1) NULL DEFAULT 'N',
    `cover_image`               char(1) NOT NULL DEFAULT 'N',
    `regdate`                   varchar(14) NULL,
    `ipaddress`                 varchar(128) NOT NULL
);

CREATE TABLE `shop_menu_option` (
    `option_no`                 bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `menu_no`                   bigint NOT NULL,
    `name`                      varchar(100) NOT NULL,
    `description`               varchar(250) NULL,
    `price`                     int NULL,
    `discount`                  int NULL,
    `sequence`                  int NOT NULL,
    `visible`                   char(1) NOT NULL DEFAULT 'Y' COMMENT '"Y":노출, "N":감추기'
);

CREATE TABLE `shop_order_menu` (
    `order_menu_no`             bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `order_no`                  bigint NOT NULL,
    `menu_no`                   bigint NOT NULL,
    `quantity`                  int NOT NULL DEFAULT 1,
    `regdate`                   varchar(14) NOT NULL
);

CREATE TABLE `shop_order_option` (
    `order_menu_no`             bigint NOT NULL,
    `option_no`                 bigint NOT NULL,
    `quantity`                  int NOT NULL DEFAULT 1
);

CREATE TABLE `shop_board` (
    `shop_no`                   bigint NOT NULL,
    `module_srl`                bigint NOT NULL
);

CREATE TABLE `data_table_status` (
    `shop_no`                   bigint NOT NULL PRIMARY KEY,
    `regdate`                   varchar(14) NOT NULL,
    `data`                      text NULL
);

CREATE TABLE `data_table_map` (
    `shop_no`                   bigint NOT NULL PRIMARY KEY,
    `regdate`                   varchar(14) NOT NULL,
    `data`                      text NULL
);

CREATE TABLE `data_menu` (
    `shop_no`                   bigint NOT NULL PRIMARY KEY,
    `regdate`                   varchar(14) NOT NULL,
    `category`                  text NULL,
    `data`                      text NULL
);

CREATE TABLE `orders` (
    `no`                        bigint NOT NULL PRIMARY KEY,
    `shop_no`                   bigint NOT NULL,
    `table_cd`                  varchar(20) NOT NULL,
    `status`                    int NOT NULL DEFAULT 0 COMMENT '0:READY, 1:USE, 2:CANCEL, 3:FINISH',
    `orders`                    text NULL
);
