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

            if (inventoryDomain == null || !inventoryDomain.Any())
            {
                return NotFound("The inventory is empty");
            }

            var inventoryDto = mapper.Map<List<InventoryDto>>(inventoryDomain);
            return Ok(inventoryDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById([FromRoute] int id)
        {
            var item = await context.Inventories.FirstOrDefaultAsync(x => x.InventoryId == id);

            if (item == null)
            {
                return NotFound("The Item is not found.");
            }

            var inventoryDto = mapper.Map<InventoryDto>(item);
            return Ok(inventoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] AddUpdateInventoryDto addInventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (addInventoryDto.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must me greater than zero" });
            }

            var inventoryDomain = mapper.Map<Inventory>(addInventoryDto);

            inventoryDomain.LastUpdated = DateTime.Now;
            await context.Inventories.AddAsync(inventoryDomain);
            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return CreatedAtAction(nameof(GetItemById), new { id = inventoryDto.InventoryId }, new
            {
                message = "Inventory added successfully.",
                inventoryDto
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateInventory([FromRoute] int id, [FromBody] AddUpdateInventoryDto updateInventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryDomain = mapper.Map<Inventory>(updateInventoryDto);

            inventoryDomain = await context.Inventories
                                .FirstOrDefaultAsync(x => x.InventoryId == id);

            if (inventoryDomain == null)
            {
                return NotFound(new { message = "Item not found in inventory" });
            }

            // Check for duplicate inventory name (excluding the current item)
            var duplicateItem = await context.Inventories
                .FirstOrDefaultAsync(i => i.InventoryName.ToLower() == updateInventoryDto.InventoryName.ToLower() && i.InventoryId != id);


            if (updateInventoryDto.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must me greater than zero" });
            }

            inventoryDomain.InventoryName = updateInventoryDto.InventoryName;
            inventoryDomain.Quantity = updateInventoryDto.Quantity;
            inventoryDomain.Category = updateInventoryDto.Category;
            inventoryDomain.LastUpdated = DateTime.Now;

            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return Ok(new
            {
                message = "Inventory Updated Successfully",
                inventoryDto
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Deleteinventory([FromRoute] int id)
        {
            var inventoryDomain = await context.Inventories.FirstOrDefaultAsync(x => x.InventoryId == id);

            if (inventoryDomain == null)
            {
                return NotFound(new { message = "Item not found in inventory" });
            }

            context.Inventories.Remove(inventoryDomain);
            await context.SaveChangesAsync();

            var inventoryDto = mapper.Map<InventoryDto>(inventoryDomain);

            return Ok(new
            {
                message = "Inventory Deleted Successfully",
                inventoryDto
            });

        }
    }
}