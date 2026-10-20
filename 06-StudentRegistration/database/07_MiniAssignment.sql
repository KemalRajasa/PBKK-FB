USE StudentRegistrationDB;
GO

-- ===== isi dari materi =====
INSERT INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
VALUES
('20231006', 'Nadia Putri',   1, '2004-02-14', 'Jl. Gubeng No. 8, Surabaya',  '081300112233'),
('20231007', 'Fajar Hidayat', 2, '2003-12-02', 'Jl. Manyar No. 15, Surabaya', '082144556677'),
('20231008', 'Rina Kartika',  4, '2004-05-30', 'Jl. Keputih No. 3, Surabaya', '085211223344');
GO

SELECT s.NIM, s.Name, p.ProgramName, s.BirthDate, s.PhoneNumber
FROM Students s
INNER JOIN Programs p ON s.ProgramId = p.ProgramId
ORDER BY s.Name;
GO
