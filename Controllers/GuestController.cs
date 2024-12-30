using Hotel_Management.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class GuestController : ControllerBase
    {
        private readonly HMDbContext context;

        public GuestController(HMDbContext context)
        {
            this.context = context;
        }

        
    }
}