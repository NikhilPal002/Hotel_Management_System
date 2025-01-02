using AutoMapper;
using Hotel_Management.Models;

namespace Hotel_Management.Mappings
{
    public class AutoMapperProfiles : Profile{
        public AutoMapperProfiles()
        {
            CreateMap<Guest,GuestDto>().ReverseMap();
            CreateMap<AddUpdateGuestDto,Guest>().ReverseMap();
            CreateMap<Room,RoomDto>().ReverseMap();
            CreateMap<AddUpdateRoomDto,Room>().ReverseMap();
            CreateMap<Booking,BookingDto>().ReverseMap();
            CreateMap<AddBookingDto,Booking>().ReverseMap();
            CreateMap<Payment,PaymentDto>().ReverseMap();
            CreateMap<AddPaymentDto,Payment>().ReverseMap();
            CreateMap<Staff,StaffDto>().ReverseMap();
            CreateMap<AddStaffDto,Staff>().ReverseMap();
            CreateMap<UpdateStaffDto,Staff>().ReverseMap();
            CreateMap<Inventory,InventoryDto>().ReverseMap();
            CreateMap<AddUpdateInventoryDto,Inventory>().ReverseMap();
            CreateMap<Billing,BillingDto>().ReverseMap();
            CreateMap<AddBillingDto,Billing>().ReverseMap();
        }
    }
}

