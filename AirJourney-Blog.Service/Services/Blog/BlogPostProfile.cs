using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.Service.Services.Blog.Dtos;
using AirJourney_Blog.Service.Services.BlogPost.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.BlogPost
{
    public class BlogPostProfile : Profile
    {
        public BlogPostProfile()
        {
            CreateMap<DAL.Models.BlogPost, BlogPostDto>().ReverseMap();

            CreateMap<DAL.Models.BlogPost, BlogDetatilsDto>()
                .ForMember(dest => dest.Title, option => option.MapFrom(src => src.Localize(src.TitleEn, src.TitleEs)))
                .ForMember(dest => dest.Content, option => option.MapFrom(src => src.Localize(src.ContentEn, src.ContentEs)))
                .ForMember(dest => dest.CategoryName, option => option
                .MapFrom(src => src.BlogCategory.Localize(src.BlogCategory.NameEn, src.BlogCategory.NameEs)));

            CreateMap<DAL.Models.BlogPost, AdminBlogDetatilsDto>()
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.BlogCategory.NameEn));


            CreateMap<DAL.Models.BlogPost, BlogListDto>()
                .ForMember(dest => dest.Title, option => option.MapFrom(src => src.Localize(src.TitleEn, src.TitleEs)))
                .ForMember(dest => dest.Excerpt, opt => opt.MapFrom(src =>
                    src.Localize(src.ContentEn, src.ContentEs).Length > 100
                        ? src.Localize(src.ContentEn, src.ContentEs).Substring(0, 100) + "..."
                        : src.Localize(src.ContentEn, src.ContentEs)))
                .ForMember(dest => dest.BlogCategoryName, opt => opt.MapFrom(src =>
                    src.BlogCategory.Localize(src.BlogCategory.NameEn, src.BlogCategory.NameEs)));


            CreateMap<DAL.Models.BlogPost, AdminBlogListDto>()
                .ForMember(dest => dest.Title, options => options.MapFrom(src => src.Localize(src.TitleEn, src.TitleEs)))
                .ForMember(dest => dest.Excerpt, opt => opt.MapFrom(src =>
                    src.Localize(
                        src.ContentEn.Length > 100 ? src.ContentEn.Substring(0, 100) + "..." : src.ContentEn,
                        src.ContentEs.Length > 100 ? src.ContentEs.Substring(0, 100) + "..." : src.ContentEs
                    )))
                .ForMember(dest => dest.BlogCategoryName, opt => opt.MapFrom(src => src.BlogCategory.NameEn));
        }
    }
}
