DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetApplicationOperationAuthorizationsByOperationId`(
	IN _OperationID BIGINT,
	IN _Count INT
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`application-operation-authorizations`
		WHERE `OperationID` = _OperationID
		LIMIT _Count;
END$$

DELIMITER ;