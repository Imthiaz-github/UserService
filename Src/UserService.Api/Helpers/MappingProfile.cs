using AutoMapper;
using UserService.Infrastructure.Entities;

namespace UserService.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, Core.Models.User>().ReverseMap().ForMember(dest=> dest.UserId, opt=>opt.Ignore());
        }
    }
}
