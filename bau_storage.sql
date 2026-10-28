-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Oct 28, 2026 at 10:17 AM
-- Server version: 8.4.7
-- PHP Version: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bau_storage`
--

-- --------------------------------------------------------

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
CREATE TABLE IF NOT EXISTS `products` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `Price` double NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `products`
--

INSERT INTO `products` (`id`, `Name`, `Description`, `Price`, `CreatedAt`, `UpdatedAt`) VALUES
(1, 'Mobil', 'Mobil MBG sss', 1000000, '2026-04-13 22:33:17', '2026-04-13 22:33:17'),
(3, 'Motor 1', 'Motor BGN', 999, '2026-04-13 22:38:02', '2026-04-13 22:38:02'),
(4, 'Bahlil', 'Goblin', 1, '2026-04-15 20:47:00', '2026-04-15 20:47:00'),
(5, 'Pigai', 'Gorila over weight', 0, '2026-04-15 21:22:00', '2026-04-15 21:22:00'),
(6, 'asasas', 'asasa', 500, '2026-04-15 21:26:09', '2026-04-15 21:26:09'),
(23, 'Bahlil 333445', 'asdasda', 55555, '2026-10-25 18:02:12', '2026-10-25 18:02:12'),
(25, 'dadsa', 'sdada', 2222, '2026-10-28 13:04:51', '2026-10-28 13:04:51');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Email` varchar(191) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Role` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `PasswordHash` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LoginAt` datetime DEFAULT NULL,
  `AccessToken` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_Email` (`Email`)
) ENGINE=MyISAM AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Email`, `Role`, `PasswordHash`, `CreatedAt`, `LoginAt`, `AccessToken`) VALUES
(20, 'jnsbstn391@gmail.com', 'Maha kuasa', 'DAAAAKHK0wRbtyGLtmN9chAAAAAA23upP/Hse0b3VkSGZ2eG/biV7bdXSHI=', '2026-10-25 17:13:54', '2026-10-28 06:02:45', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiam5zYnN0bjM5MUBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJNYWhhIGt1YXNhIiwianRpIjoiOWE0MjhmMmMtYTU0OS00N2E5LWJhZTMtMDYyNWMxMGNhZjkxIiwicmVmcmVzaFRva2VuIjoiZDU4MjU4ZmEtOWIxOC00ZjM3LWIzMjEtOTE0NDVmYzZhMzcwIiwiZXhwIjoxNzkzMTcwOTUxfQ.yE9sn7dtE9PcXRE-Rumr4itKO_o9j38VpG2b0wpGrek');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
