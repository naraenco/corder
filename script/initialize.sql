CREATE DATABASE bizorder;
CREATE USER 'bizorder'@'localhost' IDENTIFIED BY 'bizorder';
GRANT ALL PRIVILEGES ON bizorder.* TO 'bizorder'@'localhost';
FLUSH PRIVILEGES;
