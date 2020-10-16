DELIMITER $$
USE `application-authorizations`$$
CREATE PROCEDURE `GetServices`(
	IN _Count INTEGER
)
BEGIN
	SELECT *
		FROM `application-authorizations`.`services`
		LIMIT _Count;
END$$

DELIMITER ;