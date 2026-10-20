USE StudentRegistrationDB;
GO

-- ===== isi dari materi =====
-- Uji FOREIGN KEY: ProgramId 99 tidak ada di tabel Programs
INSERT INTO Students (NIM, Name, ProgramId)
VALUES ('20231099', 'Test Student', 99);

-- Uji UNIQUE: NIM 20231001 sudah ada
INSERT INTO Students (NIM, Name, ProgramId)
VALUES ('20231001', 'Mahasiswa Duplikat', 1);
