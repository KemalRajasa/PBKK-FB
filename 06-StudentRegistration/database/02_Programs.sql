USE StudentRegistrationDB;
GO

-- ===== isi dari materi =====
CREATE TABLE Programs
(
    ProgramId INT IDENTITY(1,1)
        CONSTRAINT PK_Programs PRIMARY KEY,
    ProgramCode VARCHAR(20) NOT NULL
        CONSTRAINT UQ_Programs_ProgramCode UNIQUE,
    ProgramName VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL
        CONSTRAINT DF_Programs_IsActive DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Programs_CreatedAt DEFAULT SYSDATETIME()
);
GO

INSERT INTO Programs (ProgramCode, ProgramName)
VALUES
('TI',  'Teknik Informatika'),
('SI',  'Sistem Informasi'),
('MNJ', 'Manajemen'),
('TE',  'Teknik Elektro'),
('DKV', 'Desain Komunikasi Visual');
GO

SELECT * FROM Programs;
GO
