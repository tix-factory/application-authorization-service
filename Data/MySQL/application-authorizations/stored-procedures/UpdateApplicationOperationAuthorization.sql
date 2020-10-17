DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `UpdateApplicationOperationAuthorization`(
	IN _ApplicationID BIGINT,
	IN _OperationID BIGINT,
	IN _ID BIGINT
)
BEGIN
	UPDATE `application-authorizations`.`application-operation-authorizations`
	SET
		`ApplicationID` = _ApplicationID,
		`OperationID` = _OperationID,
		`Updated` = UTC_Timestamp()
	WHERE (`ID` = _ID)
	LIMIT 1;
END$$

DELIMITER ;