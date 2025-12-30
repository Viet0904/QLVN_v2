INSERT INTO [dbo].[UsGroup] (Id, Name, Note, RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES ('001', 'Administrator', 'Admin Group', 1, GETDATE(), NULL, GETDATE(), NULL);
INSERT INTO [dbo].[UsUser] (Id, GroupId, Name, Gender, UserName, Password, Email, Phone, CMND, Address, Image, Note, RowStatus, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Theme)
VALUES ('00100001', '001', 'Admin', NULL, 'admin', 'drgrMqLgOrVxRqQjh3Ba5g==', NULL, NULL, NULL, NULL, NULL, NULL, 1, GETDATE(), '00100001', GETDATE(), '00100001', '');