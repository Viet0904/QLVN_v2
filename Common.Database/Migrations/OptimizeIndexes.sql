-- Index cho khóa ngoại để Join cực nhanh
CREATE INDEX IX_UsUser_GroupId ON UsUser(GroupId);

-- Index cho các cột thường xuyên tìm kiếm và sắp xếp
CREATE INDEX IX_UsUser_UserName ON UsUser(UserName);
CREATE INDEX IX_UsUser_RowStatus_CreatedAt ON UsUser(RowStatus, CreatedAt DESC);

-- Index cho bảng Group để tìm kiếm tên nhanh
CREATE INDEX IX_UsGroup_Name ON UsGroup(Name);
