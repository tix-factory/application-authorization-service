USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `client-keys` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ClientID` BIGINT NOT NULL,
	`KeyHash` VARCHAR(64) NOT NULL,
	`Enabled` BIT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,

	PRIMARY KEY (`ID`),
	FOREIGN KEY `FK_ClientKeys_Clients_ClientID` (`ClientID`) REFERENCES `clients`(`ID`) ON DELETE CASCADE
);
