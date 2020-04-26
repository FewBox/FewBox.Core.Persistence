DROP TABLE IF EXISTS `app`;
CREATE TABLE `app` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Key` varchar(45) DEFAULT NULL,
  `Char36ID` char(36) NOT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
);
DROP TABLE IF EXISTS `app_recycle`;
CREATE TABLE `app_recycle` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Key` varchar(45) DEFAULT NULL,
  `Char36ID` char(36) NOT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
);