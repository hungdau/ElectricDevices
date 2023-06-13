namespace TechZone.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRevenueStatistic : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("GetRevenueStatisticByDate",
                p => new
                {
                    fromDate = p.String(),
                    toDate = p.String()
                }
                ,
                @"
                select
                CAST(o.CreatedDate AS date) as Date,
                SUM(od.Quantity*od.Price) as Revenues,
                SUM((od.Quantity*od.Price)-(od.Quantity*p.OriginalPrice)) as Benefit
                from Orders o
                inner join OrderDetails od
                on o.ID = od.OrderId
                inner join Products p
                on od.ProductID  = p.ID
                where o.CreatedDate <= cast(@toDate as date) and o.CreatedDate >= cast(@fromDate as date)
                group by CAST(o.CreatedDate AS date)"
                );

            CreateStoredProcedure("GetRevenueStatisticByMonth",
                p => new
                {
                    fromDate = p.String(),
                    toDate = p.String()
                }
                ,
                @"
                select
                DATEPART(month, o.CreatedDate) as Month,
                SUM(od.Quantity*od.Price) as Revenues,
                SUM((od.Quantity*od.Price)-(od.Quantity*p.OriginalPrice)) as Benefit
                from Orders o
                inner join OrderDetails od
                on o.ID = od.OrderId
                inner join Products p
                on od.ProductID  = p.ID
                where o.CreatedDate <= cast(@toDate as date) and o.CreatedDate >= cast(@fromDate as date)
                group by DATEPART(month, o.CreatedDate)"
                );
        }

        public override void Down()
        {
            DropStoredProcedure("dbo.GetRevenueStatisticByDate");
            DropStoredProcedure("dbo.GetRevenueStatisticByMonth");
        }
    }
}
