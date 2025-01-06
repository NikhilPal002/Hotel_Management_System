using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class StaffController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public StaffController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(){

            var staffDomain = await context.Staffs.Include("User").ToListAsync();

            if(staffDomain == null){
                return NotFound("No staff found");
            }

            var staffDto = mapper.Map<List<StaffDto>>(staffDomain);

            return Ok(staffDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] AddStaffDto addStaffDto){
            var staffDomain = mapper.Map<Staff>(addStaffDto);

            if(staffDomain.Age < 18 ){
                return BadRequest("The age must be greater than 18 years");
            }

            await context.Staffs.AddAsync(staffDomain);
            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return Ok(staffDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStaff([FromRoute] int id, [FromBody] UpdateStaffDto updateStaffDto){
            var staffDomain = mapper.Map<Staff>(updateStaffDto);

            staffDomain = await context.Staffs.FirstOrDefaultAsync(x=>x.StaffId==id);

            if(staffDomain == null){
                return NotFound("No staff available with this id");
            }

            if(staffDomain.Age < 18 ){
                return BadRequest("The age must be greater than 18 years");
            }

            staffDomain.FullName = updateStaffDto.FullName;
            staffDomain.Age = updateStaffDto.Age;
            staffDomain.SAddress = updateStaffDto.SAddress;
            staffDomain.Salary = updateStaffDto.Salary;
            staffDomain.Designation = updateStaffDto.Designation;
            staffDomain.Email = updateStaffDto.Email;

            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return Ok(staffDto);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteStaff([FromRoute] int id){
            var staffDomain = await context.Staffs.FirstOrDefaultAsync(x=>x.StaffId==id);

            if(staffDomain == null){
                return NotFound("No staff available with this id");
            }

            context.Staffs.Remove(staffDomain);
            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return Ok(staffDto);
        }
    }

}