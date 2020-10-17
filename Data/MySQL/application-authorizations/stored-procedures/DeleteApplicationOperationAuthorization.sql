DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `DeleteApplicationOperationAuthorization`(
	IN _ID BIGINT
)
BEGIN
	DELETE
		FROM `application-authorizations`.`application-operation-authorizations`
		WHERE (`ID` = _ID)
		LIMIT 1;
END$$

DELIMITER ;