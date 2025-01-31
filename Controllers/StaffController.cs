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
        public async Task<IActionResult> GetAll()
        {

            var staffDomain = await context.Staffs.ToListAsync();

            if (staffDomain == null || !staffDomain.Any())
            {
                return NotFound(new { message = "No staff found" });
            }

            var staffDto = mapper.Map<List<StaffDto>>(staffDomain);

            return Ok(staffDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById([FromRoute] int id)
        {
            var staff = await context.Staffs.FirstOrDefaultAsync(x => x.StaffId == id);

            if (staff == null)
            {
                return NotFound(new { message = "The staff member was not found." });
            }

            var staffDto = mapper.Map<StaffDto>(staff);
            return Ok(staffDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] AddStaffDto addStaffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staffDomain = mapper.Map<Staff>(addStaffDto);

            await context.Staffs.AddAsync(staffDomain);
            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return CreatedAtAction(nameof(GetStaffById), new { id = staffDto.StaffId }, new
            {
                message = "Staff added successfully",
                staffDto
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStaff([FromRoute] int id, [FromBody] UpdateStaffDto updateStaffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staffDomain = mapper.Map<Staff>(updateStaffDto);

            staffDomain = await context.Staffs.FirstOrDefaultAsync(x => x.StaffId == id);

            if (staffDomain == null)
            {
                return NotFound(new { message = "Staff not found." });
            }

            staffDomain.FullName = updateStaffDto.FullName;
            staffDomain.Age = updateStaffDto.Age;
            staffDomain.SAddress = updateStaffDto.SAddress;
            staffDomain.Salary = updateStaffDto.Salary;
            staffDomain.Designation = updateStaffDto.Designation;
            staffDomain.Email = updateStaffDto.Email;

            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return Ok(new
            {
                message = "Staff Updated Successfully",
                staffDto
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteStaff([FromRoute] int id)
        {
            var staffDomain = await context.Staffs.FirstOrDefaultAsync(x => x.StaffId == id);

            if (staffDomain == null)
            {
                return NotFound(new { message = "Staff not found." });
            }

            context.Staffs.Remove(staffDomain);
            await context.SaveChangesAsync();

            var staffDto = mapper.Map<StaffDto>(staffDomain);
            return Ok(new
            {
                message = "Staff Updated Successfully",
                staffDto
            });
        }
    }

}