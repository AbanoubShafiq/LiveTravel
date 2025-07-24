using AirJourney_Blog.Service.Services.Category.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Category
{
    public class BlogCategoryProfile: Profile
    {
        public BlogCategoryProfile()
        {
            // mapping dto for create 
            CreateMap<DAL.Models.BlogCategory, CategoryDto>().ReverseMap();

            // mapping dto for get all 
            CreateMap<DAL.Models.BlogCategory, AdminCategoryListDto>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Localize(src.NameEn, src.NameEs)))
                .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Localize(src.DescriptionEn, src.DescriptionEs)))
                .ReverseMap();

            // mapping for get by id dto 
            CreateMap<DAL.Models.BlogCategory, CategoryDetailDto>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Localize(src.NameEn, src.NameEs)))
                .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Localize(src.DescriptionEn, src.DescriptionEs)))
                .ForMember(dest => dest.NoOfBlogs, options => options
                .MapFrom(src => src.Blogs.Count()))
                .ReverseMap();

            CreateMap<DAL.Models.BlogCategory, AdminCategoryDetailDto>()
                .ForMember(dest => dest.NoOfBlogs, options => options
                .MapFrom(src => src.Blogs.Count()))
                .ReverseMap();


            CreateMap<DAL.Models.BlogCategory, CategoryDropdownDto>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Localize(src.NameEn, src.NameEs)));

            CreateMap<DAL.Models.BlogCategory, AdminCategoryDropdownDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn));

        }
    }
}
