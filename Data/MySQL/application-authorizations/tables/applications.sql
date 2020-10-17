USE `application-authorizations`;

CREATE TABLE IF NOT EXISTS `applications` (
	`ID` BIGINT NOT NULL AUTO_INCREMENT,
	`Name` VARCHAR(100) NOT NULL,
	`Updated` DATETIME NOT NULL,
	`Created` DATETIME NOT NULL,

	PRIMARY KEY (`ID`),
	CONSTRAINT `UC_Name` UNIQUE(`Name`)
);
