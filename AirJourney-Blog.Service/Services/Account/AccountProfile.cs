using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.Service.Services.Account.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJourney_Blog.Service.Services.Account
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterDto, AppUser>().ReverseMap();

            CreateMap<AppUser, AccountDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

        }
    }
}
