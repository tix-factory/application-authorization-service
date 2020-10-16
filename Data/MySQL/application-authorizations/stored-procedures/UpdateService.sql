DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateService`(
	IN _Name VARBINARY(50),
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`services`
	SET
		`Name` = _Name,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;