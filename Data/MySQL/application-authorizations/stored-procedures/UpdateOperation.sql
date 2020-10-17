DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateOperation`(
	IN _ApplicationID BIGINT,
	IN _Name VARBINARY(50),
	IN _Enabled BIT,
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`operations`
	SET
		`ApplicationID` = _ApplicationID,
		`Name` = _Name,
		`Enabled` = _Enabled,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;