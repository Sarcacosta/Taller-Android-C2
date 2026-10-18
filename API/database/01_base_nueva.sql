-- Solo para una base nueva. No reemplaza una base con usuarios existentes.
CREATE DATABASE IF NOT EXISTS ucompensar CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE ucompensar;
CREATE TABLE Usuarios (
  Id INT NOT NULL AUTO_INCREMENT,
  Nombre VARCHAR(100) NOT NULL,
  Apellido VARCHAR(100) NOT NULL,
  Correo VARCHAR(150) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  FechaNacimiento DATE NOT NULL,
  Universidad VARCHAR(150) NOT NULL,
  Semestre INT NOT NULL,
  Activo TINYINT(1) NOT NULL DEFAULT 1,
  FechaCreacion DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (Id),
  UNIQUE KEY IX_Usuarios_Correo (Correo),
  CONSTRAINT CK_Usuarios_Semestre CHECK (Semestre BETWEEN 1 AND 10)
);
