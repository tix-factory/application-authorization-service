USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `application-operation-authorizations` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`ApplicationID` BIGINT NOT NULL,
	`OperationID` BIGINT NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,
	
	PRIMARY KEY (`ID`),
	CONSTRAINT `UC_ApplicationIDOperationID` UNIQUE(`ApplicationID`, `OperationID`),
	FOREIGN KEY `FK_ApplicationOperationAuthorizations_Applications_ApplicationID` (`ApplicationID`) REFERENCES `applications`(`ID`) ON DELETE CASCADE,
	FOREIGN KEY `FK_ApplicationOperationAuthorizations_Operations_OperationID` (`OperationID`) REFERENCES `operations`(`ID`) ON DELETE CASCADE
);
