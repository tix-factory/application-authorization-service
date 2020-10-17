DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `InsertApplicationOperationAuthorization`(
	IN _ApplicationID BIGINT,
	IN _OperationID BIGINT
)
BEGIN
	INSERT INTO `application-authorizations`.`application-operation-authorizations`
	(
		`ApplicationID`,
		`OperationID`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_ApplicationID,
		_OperationID,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END$$

DELIMITER ;