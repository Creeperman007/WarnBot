CREATE TABLE `permissions` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `guild` bigint(20) NOT NULL,
 `user` bigint(20) NOT NULL,
 `kick` tinyint(1) NOT NULL,
 `ban` tinyint(1) NOT NULL,
 PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `warnings` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `guild` bigint(20) NOT NULL,
 `user` bigint(20) NOT NULL,
 `warnsCurrent` int(11) NOT NULL DEFAULT '0',
 `warnsTotal` int(11) NOT NULL DEFAULT '0',
 `kicks` int(11) NOT NULL DEFAULT '0',
 `kickReason` text CHARACTER SET utf8 COLLATE utf8_bin,
 `banReason` text,
 PRIMARY KEY (`id`),
 UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
