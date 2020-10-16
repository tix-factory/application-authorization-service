USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `client-service-authorizations` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ClientID` BIGINT NOT NULL,
	`ServiceID` BIGINT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,
	
	PRIMARY KEY (`ID`),
	FOREIGN KEY `FK_ClientServiceAuthorizations_Clients_ClientID` (`ClientID`) REFERENCES `clients`(`ID`) ON DELETE CASCADE,
	FOREIGN KEY `FK_ClientServiceAuthorizations_Services_ServiceID` (`ServiceID`) REFERENCES `services`(`ID`) ON DELETE CASCADE
);
