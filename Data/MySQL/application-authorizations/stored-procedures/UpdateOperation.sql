DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateOperation`(
	IN _ServiceID BIGINT,
	IN _Name VARBINARY(50),
	IN _Enabled BIT,
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`operations`
	SET
		`ServiceID` = _ServiceID,
		`Name` = _Name,
		`Enabled` = _Enabled,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;