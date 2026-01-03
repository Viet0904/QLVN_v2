-- Script chèn 50,000 dòng dữ liệu mẫu cho UsGroup
DECLARE @cnt INT = 1;
DECLARE @id NVARCHAR(50);
DECLARE @name NVARCHAR(255);

WHILE @cnt <= 50000
BEGIN
    SET @id = 'GRP' + RIGHT('00000' + CAST(@cnt AS NVARCHAR(10)), 5);
    SET @name = N'Nhóm người dùng số ' + CAST(@cnt AS NVARCHAR(10));
    
    INSERT INTO [UsGroup] ([Id], [Name], [Note], [RowStatus], [CreatedAt], [CreatedBy])
    VALUES (@id, @name, N'Dữ liệu test tự động ' + CAST(@cnt AS NVARCHAR(10)), 1, GETDATE(), 'admin');
    
    SET @cnt = @cnt + 1;
END
GO
