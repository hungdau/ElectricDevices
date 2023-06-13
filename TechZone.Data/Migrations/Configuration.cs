namespace TechZone.Data.Migrations
{
    using Common;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TechZone.Data.TechZoneDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TechZone.Data.TechZoneDbContext context)
        {
            //CreateUser(context);
            //CreateProductCategorySample(context);
            //CreateSlide(context);
            //CreatePage(context);

            //CreateContactDetail(context);

            //CreateConfigTitle(context);
            //  This method will be called after migrating to the latest version.
        }

        private void CreateConfigTitle(TechZoneDbContext context)
        {
            if (!context.SystemConfigs.Any(x => x.Code == "HomeTitle"))
            {
                context.SystemConfigs.Add(new SystemConfig()
                {
                    Code = "HomeTitle",
                    ValueString = "Trang chủ TechZone",

                });
            }
            if (!context.SystemConfigs.Any(x => x.Code == "HomeMetaKeyword"))
            {
                context.SystemConfigs.Add(new SystemConfig()
                {
                    Code = "HomeMetaKeyword",
                    ValueString = "Trang chủ TechZone",

                });
            }
            if (!context.SystemConfigs.Any(x => x.Code == "HomeMetaDescription"))
            {
                context.SystemConfigs.Add(new SystemConfig()
                {
                    Code = "HomeMetaDescription",
                    ValueString = "Trang chủ TechZone",

                });
            }
        }

        private void CreateUser(TechZoneDbContext context)
        {
            if (!context.Users.Any())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new TechZoneDbContext()));

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new TechZoneDbContext()));

                var user = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "techzone@gmail.com",
                    EmailConfirmed = true,
                    BirthDay = DateTime.Now,
                    FullName = "Admin"

                };

                manager.Create(user, "123456");

                if (!roleManager.Roles.Any())
                {
                    roleManager.Create(new IdentityRole { Name = "Admin" });
                    roleManager.Create(new IdentityRole { Name = "User" });
                }

                var adminUser = manager.FindByEmail("techzone.international@gmail.com");

                manager.AddToRoles(adminUser.Id, new string[] { "Admin", "User" });
            }
        }

        private void CreateProductCategorySample(TechZone.Data.TechZoneDbContext context)
        {
            if (context.ProductCategories.Count() == 0)
            {
                List<ProductCategory> listProductCategory = new List<ProductCategory>()
            {
                new ProductCategory() { Name="Điện lạnh",Alias="dien-lanh",Status=true },
                 new ProductCategory() { Name="Viễn thông",Alias="vien-thong",Status=true },
                  new ProductCategory() { Name="Đồ gia dụng",Alias="do-gia-dung",Status=true },
                   new ProductCategory() { Name="Mỹ phẩm",Alias="my-pham",Status=true }
            };
                context.ProductCategories.AddRange(listProductCategory);
                context.SaveChanges();
            }
        }

        private void CreateFooter(TechZoneDbContext context)
        {
            if (context.Footers.Count(x => x.ID == CommonConstants.DefaultFooterId) == 0)
            {
            }
        }

        private void CreateSlide(TechZoneDbContext context)
        {
            if (context.Slides.Count() == 0)
            {
                List<Slide> listSlide = new List<Slide>()
                {
                    new Slide() {
                        Name ="Slide 1",
                        DisplayOrder =1,
                        Status =true,
                        Url ="#",
                        Image ="/Assets/client/images/bag.jpg",
                        Content =@"	<h2>FLAT 50% 0FF</h2>
                                <label>FOR ALL PURCHASE <b>VALUE</b></label>
                                <p>Lorem ipsum dolor sit amet, consectetur
                            adipisicing elit, sed do eiusmod tempor incididunt ut labore et </ p >
                        <span class=""on-get"">GET NOW</span>" },
                    new Slide() {
                        Name ="Slide 2",
                        DisplayOrder =2,
                        Status =true,
                        Url ="#",
                        Image ="/Assets/client/images/bag1.jpg",
                    Content=@"<h2>FLAT 50% 0FF</h2>
                                <label>FOR ALL PURCHASE <b>VALUE</b></label>

                                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et </ p >

                                <span class=""on-get"">GET NOW</span>"},
                };
                context.Slides.AddRange(listSlide);
                context.SaveChanges();
            }
        }

        private void CreatePage(TechZoneDbContext context)
        {
            if (context.Pages.Count() == 0)
            {
                try
                {
                    var page = new Page()
                    {
                        Name = "Giới thiệu",
                        Alias = "gioi-thieu",
                        Content = @"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium ",
                        Status = true
                    };
                    context.Pages.Add(page);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Trace.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation error.");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Trace.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        }
                    }
                }
            }
        }

        private void CreateContactDetail(TechZoneDbContext context)
        {
            if (context.ContactDetails.Count() == 0)
            {
                try
                {
                    var contactDetail = new TechZone.Model.Models.ContactDetail()
                    {
                        Name = "Shop công nghệ TECHZONE",
                        Address = "Xuân Phương",
                        Email = "techzone@gmail.com",
                        Lat = 21.0538776,
                        Lng = 105.735033,
                        Phone = "0333333333",
                        Website = "http://techzone.com",
                        Other = "",
                        Status = true
                    };
                    context.ContactDetails.Add(contactDetail);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Trace.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation error.");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Trace.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                        }
                    }
                }
            }
        }
    }
}