USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `client-operation-authorizations` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ClientID` BIGINT NOT NULL,
	`OperationID` BIGINT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,
	
	PRIMARY KEY (`ID`),
	FOREIGN KEY `FK_ClientOperationAuthorizations_Clients_ClientID` (`ClientID`) REFERENCES `clients`(`ID`) ON DELETE CASCADE,
	FOREIGN KEY `FK_ClientOperationAuthorizations_Operations_OperationID` (`OperationID`) REFERENCES `operations`(`ID`) ON DELETE CASCADE
);
