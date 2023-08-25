using AutoMapper;

namespace Identity.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegistrationModel, ApplicationUser>()
            .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        CreateMap<RoleRegistrationModel, ApplicationRole>();
        CreateMap<ApplicationRole, RoleModel>();

    }
}