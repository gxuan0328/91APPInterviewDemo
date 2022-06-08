/*
 Navicat Premium Data Transfer

 Source Server         : QAQ
 Source Server Type    : MariaDB
 Source Server Version : 100703
 Source Host           : localhost:3306
 Source Schema         : ManagementCenter

 Target Server Type    : MariaDB
 Target Server Version : 100703
 File Encoding         : 65001

 Date: 05/06/2022 23:55:32
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for Order
-- ----------------------------
DROP TABLE IF EXISTS `Order`;
CREATE TABLE `Order`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Buyer_Id` int(11) NOT NULL,
  `Seller_Id` int(11) NOT NULL,
  `Address` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `PaymentType_Id` int(11) NOT NULL,
  `OrderStatusType_Id` int(11) NOT NULL DEFAULT 1,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for OrderStatusType
-- ----------------------------
DROP TABLE IF EXISTS `OrderStatusType`;
CREATE TABLE `OrderStatusType`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(255) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for Order_Product_Relation
-- ----------------------------
DROP TABLE IF EXISTS `Order_Product_Relation`;
CREATE TABLE `Order_Product_Relation`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Order_Id` int(11) NOT NULL,
  `Product_Id` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 12 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for PaymentType
-- ----------------------------
DROP TABLE IF EXISTS `PaymentType`;
CREATE TABLE `PaymentType`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for Product
-- ----------------------------
DROP TABLE IF EXISTS `Product`;
CREATE TABLE `Product`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Cost` int(11) NOT NULL,
  `Price` int(11) NOT NULL,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for User
-- ----------------------------
DROP TABLE IF EXISTS `User`;
CREATE TABLE `User`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Account` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Password` varchar(20) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Admin_Id` int(11) NOT NULL,
  `CreateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `UpdateTime` datetime NOT NULL DEFAULT utc_timestamp(),
  `Archive` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb3 COLLATE = utf8mb3_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
