using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechZone.Model.Models;
using TechZone.Web.Models;

namespace TechZone.Web.Mappings
{
    public interface IMappingService
    {
        IMapper Mapper { get; }
    }

    public class MappingService : IMappingService
    {
        public IMapper Mapper { get; }

        public MappingService()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostViewModel>();
                cfg.CreateMap<PostCategory, PostCategoryViewModel>();
                cfg.CreateMap<Tag, TagViewModel>();

                cfg.CreateMap<ProductCategory, ProductCategoryViewModel>();
                cfg.CreateMap<Product, ProductViewModel>();
                cfg.CreateMap<ProductTag, ProductTagViewModel>();
                cfg.CreateMap<Footer, FooterViewModel>();
                cfg.CreateMap<Slide, SlideViewModel>();
                cfg.CreateMap<Page, PageViewModel>();
                cfg.CreateMap<ContactDetail, ContactDetailViewModel>();
                cfg.CreateMap<ApplicationGroup, ApplicationGroupViewModel>();
                cfg.CreateMap<ApplicationRole, ApplicationRoleViewModel>();
                cfg.CreateMap<ApplicationUser, ApplicationUserViewModel>();
                cfg.CreateMap<Order, OrderViewModel>();
                cfg.CreateMap<OrderDetail, OrderDetailViewModel>();
            });

            Mapper = config.CreateMapper();
        }
    }
}