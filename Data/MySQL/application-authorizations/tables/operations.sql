USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `operations` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ServiceID` BIGINT NOT NULL,
	`Name` VARCHAR(100) NOT NULL,
	`Enabled` BIT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,

	PRIMARY KEY (`ID`),
	FOREIGN KEY `FK_Operations_Services_ServiceID` (`ServiceID`) REFERENCES `services`(`ID`) ON DELETE CASCADE
);
