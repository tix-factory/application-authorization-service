USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `service-keys` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ServiceID` BIGINT NOT NULL,
	`KeyHash` VARCHAR(64) NOT NULL,
	`Enabled` BIT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,

	PRIMARY KEY (`ID`),
	FOREIGN KEY `FK_ServiceKeys_Services_ServiceID` (`ServiceID`) REFERENCES `services`(`ID`) ON DELETE CASCADE
);
