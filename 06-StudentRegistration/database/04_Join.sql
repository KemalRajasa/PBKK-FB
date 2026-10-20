USE StudentRegistrationDB;
GO

SELECT
    s.StudentId, s.NIM, s.Name,
    p.ProgramCode, p.ProgramName,
    s.BirthDate, s.PhoneNumber
FROM Students s
INNER JOIN Programs p ON s.ProgramId = p.ProgramId;
GO
