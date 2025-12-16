/*IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usp_GetTopNSellingProductsByDate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Usp_GetTopNSellingProductsByDate]
GO*/
CREATE PROCEDURE [dbo].[Usp_GetTopNSellingProductsByDate]
    @startDate datetime, 
    @endDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    
    -- আপনার মূল লজিক (UnitSold এবং select top 5...)
    WITH UnitSold AS
    ( 
        SELECT od.ProductId, SUM(od.Quantity) AS TotalUnitSold 
        FROM [Order] o
        JOIN OrderDetail od ON o.Id = od.OrderId
        WHERE o.IsPaid = 1 
          AND o.IsDeleted = 0 
          AND o.CreateDate BETWEEN @startDate AND @endDate
        GROUP BY od.ProductId 
    )
    SELECT TOP 5 b.ProductName, 
                b.CompanyName, 
                b.[Image], 
                us.TotalUnitSold
    FROM UnitSold us
    JOIN [Product] b ON us.ProductId = b.Id
    ORDER BY us.TotalUnitSold DESC;
END
/*
-----kotoguli product paid kora hoise
update [Order] set IsPaid = 1
---Totalunitsold
EXEC [dbo].[Usp_GetTopNSellingProductsByDate] 
    @startDate = '2024-10-01', 
    @endDate = '2025-10-24'
*/