DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetOperationsByServiceId`(
	IN _ServiceID BIGINT,
	IN _Count INTEGER
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`operations`
		WHERE `ServiceID` = _ServiceID
		LIMIT _Count;
END$$

DELIMITER ;