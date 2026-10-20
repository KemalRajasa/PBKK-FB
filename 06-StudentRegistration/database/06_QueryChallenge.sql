USE StudentRegistrationDB;
GO

-- ===== isi dari materi =====
-- Mahasiswa Teknik Informatika
SELECT s.NIM, s.Name, p.ProgramName
FROM Students s
INNER JOIN Programs p ON s.ProgramId = p.ProgramId
WHERE p.ProgramCode = 'TI';
GO

-- Jumlah seluruh mahasiswa
SELECT COUNT(*) AS TotalMahasiswa FROM Students;
GO

-- Jumlah mahasiswa per program studi
SELECT p.ProgramName, COUNT(s.StudentId) AS JumlahMahasiswa
FROM Programs p
LEFT JOIN Students s ON p.ProgramId = s.ProgramId
GROUP BY p.ProgramId, p.ProgramName;
GO
