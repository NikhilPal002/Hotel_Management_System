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
    public class InventoryController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public InventoryController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventoryDomain = await context.Inventories.ToListAsync();

            if (inventoryDomain == null)
            {
                return NotFound("The inventory is empty");
            }

            var inventoryDto = mapper.Map<List<InventoryDto>>(inventoryDomain);
            return Ok(inventoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] AddUpdateInventoryDto addInventoryDto)
        {

            var inventoryDomain = mapper.Map<Inventory>(addInventoryDto);

            if (inventoryDomain.Quantity <= 0)
            {
                return BadRequest("Quantity must me greater than zero");
            }

            inventoryDomain.LastUpdated = DateTime.Now;
            await context.Inventories.AddAsync(inventoryDomain);
            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return Ok(inventoryDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateInventory([FromRoute] int id, [FromBody] AddUpdateInventoryDto updateInventoryDto)
        {

            var inventoryDomain = mapper.Map<Inventory>(updateInventoryDto);

            inventoryDomain = await context.Inventories
                                .FirstOrDefaultAsync(x => x.InventoryId == id);

            if (inventoryDomain == null)
            {
                return NotFound("Item not found");
            }

            if (inventoryDomain.Quantity <= 0)
            {
                return BadRequest("Quantity must me greater than zero");
            }

            inventoryDomain.InventoryName = updateInventoryDto.InventoryName;
            inventoryDomain.Quantity = updateInventoryDto.Quantity;
            inventoryDomain.Category = updateInventoryDto.Category;
            inventoryDomain.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return Ok(inventoryDto);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Deleteinventory([FromRoute] int id){
            var inventoryDomain = await context.Inventories.FirstOrDefaultAsync(x=>x.InventoryId == id);

            if(inventoryDomain == null){
                return BadRequest("Item not found in the inventory");
            }

            context.Inventories.Remove(inventoryDomain);
            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return Ok("The item has been removed from inventory");

        }
    }
}