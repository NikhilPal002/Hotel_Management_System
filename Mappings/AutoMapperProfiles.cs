using AutoMapper;
using Hotel_Management.Models;

namespace Hotel_Management.Mappings
{
    public class AutoMapperProfiles : Profile{
        public AutoMapperProfiles()
        {
            CreateMap<Guest,GuestDto>().ReverseMap();
            CreateMap<AddUpdateGuestDto,Guest>().ReverseMap();

           
        }
    }
}


 // CreateMap<Region, RegionDto>().ReverseMap();
            // CreateMap<AddRegionRequestDto,Region>().ReverseMap();
            // CreateMap<UpdateRegionRequestDto,Region>().ReverseMap();

            // CreateMap<AddWalksRequestDto,Walk>().ReverseMap();
            // CreateMap<Walk,WalkDto>().ReverseMap();
            // CreateMap<Difficulty,DifficultyDto>().ReverseMap();
            // CreateMap<UpdateWalkRequestDto,Walk>().ReverseMap();