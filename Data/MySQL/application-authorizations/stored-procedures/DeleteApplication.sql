DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `DeleteApplication`(
	IN _ID BIGINT
)
BEGIN
	DELETE
		FROM `application-authorizations`.`applications`
		WHERE (`ID` = _ID)
		LIMIT 1;
END$$

DELIMITER ;