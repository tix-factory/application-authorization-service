DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `DeleteApplicationKey`(
	IN _ID BIGINT
)
BEGIN
	DELETE
		FROM `application-authorizations`.`application-keys`
		WHERE (`ID` = _ID)
		LIMIT 1;
END$$

DELIMITER ;