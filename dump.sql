CREATE TABLE `accounts` (
  `account_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(13) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `login_status` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `gender` tinyint(3) unsigned NOT NULL DEFAULT '10',
  `second_password` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `last_known_ip` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `gm_level` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`account_id`) USING BTREE,
  UNIQUE KEY `accounts_username_unique` (`username`),
  KEY `accounts_username_index` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=ascii COLLATE=ascii_bin;

CREATE TABLE `characters` (
  `account_id` int(10) unsigned NOT NULL,
  `character_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(13) COLLATE ascii_bin NOT NULL,
  `gender` tinyint(3) unsigned NOT NULL,
  `skin` tinyint(3) unsigned NOT NULL,
  `face` int(10) unsigned NOT NULL,
  `hair` int(10) unsigned NOT NULL,
  `level` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `job` smallint(5) unsigned NOT NULL DEFAULT '0',
  `str` smallint(5) unsigned NOT NULL DEFAULT '4',
  `dex` smallint(5) unsigned NOT NULL DEFAULT '4',
  `int` smallint(5) unsigned NOT NULL DEFAULT '4',
  `luk` smallint(5) unsigned NOT NULL DEFAULT '4',
  `hp` smallint(5) unsigned NOT NULL DEFAULT '50',
  `max_hp` smallint(5) unsigned NOT NULL DEFAULT '50',
  `mp` smallint(5) unsigned NOT NULL DEFAULT '5',
  `max_mp` smallint(5) unsigned NOT NULL DEFAULT '5',
  `ability_points` smallint(5) unsigned NOT NULL DEFAULT '0',
  `exp` int(10) unsigned NOT NULL DEFAULT '0',
  `popularity` smallint(5) unsigned NOT NULL DEFAULT '0',
  `field_id` int(10) unsigned NOT NULL DEFAULT '10000',
  `portal` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `gm_level` tinyint(3) unsigned not null default '0',
  `money` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`character_id`) USING BTREE,
  UNIQUE KEY `username_UNIQUE` (`username`),
  KEY `FK_char_account_id_idx` (`account_id`),
  CONSTRAINT `FK_char_account_id` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`account_id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=ascii COLLATE=ascii_bin;

CREATE TABLE `items` (
  `generated_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(10) unsigned NOT NULL,
  `character_id` int(10) unsigned NOT NULL,
  `item_id` int(10) unsigned NOT NULL,
  `bag_index` smallint(6) NOT NULL,
  `quantity` smallint(5) unsigned NOT NULL,
  `cash_sn` bigint(20) unsigned NOT NULL,
  `date_expire` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`generated_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=ascii COLLATE=ascii_bin;

CREATE TABLE  `skill_records` (
  `character_id` int(10) unsigned NOT NULL,
  `skill_id` int(10) unsigned NOT NULL,
  `date_expire` bigint(20) unsigned NOT NULL,
  `master_level` int(10) unsigned NOT NULL,
  KEY `skill_records_user_id_idx` (`character_id`) USING BTREE,
  CONSTRAINT `FK_skill_records_user_id` FOREIGN KEY (`character_id`) REFERENCES `characters` (`character_id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=ascii COLLATE=ascii_bin;