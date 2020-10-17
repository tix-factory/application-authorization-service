USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `application-keys` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ApplicationID` BIGINT NOT NULL,
	`KeyHash` VARCHAR(64) NOT NULL,
	`Enabled` BIT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,

	PRIMARY KEY (`ID`),
	CONSTRAINT `UC_KeyHash` UNIQUE(`KeyHash`),
	FOREIGN KEY `FK_ApplicationKeys_Applications_ApplicationID` (`ApplicationID`) REFERENCES `applications`(`ID`) ON DELETE CASCADE
);