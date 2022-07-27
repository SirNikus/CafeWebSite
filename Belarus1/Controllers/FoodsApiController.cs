using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Belarus1;

namespace Belarus1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsApiController : ControllerBase
    {
        private readonly AUTHDBContext _context;

        public FoodsApiController(AUTHDBContext context)
        {
            _context = context;
        }

        public class Fods
		{
            public int id { get; set; }
            public string name { get; set; }
            public float price { get; set; }
            public float weight { get; set; }
            public List<string> ingredients { get; set; }
            public string type { get; set; }
            public byte[] image { get; set; }

        }

        List<Fods> fods = new List<Fods>();
        private List<Fods> setFods()
		{
            foreach(var item in _context.Foods)
			{
                fods.Add(new Fods
                {
                    id = item.Id,
                    name = item.Name,
                    price = (float)item.Price,
                    weight = (float)item.Weight,
                    image = item.Image,
                    ingredients = _context.FoodIngredients.Where(p=>p.FoodId==item.Id).Select(p => p.Ingredient.Name).ToList(),
                    type = _context.TypeOfFoods.Where(p=>p.Id==item.TypeId).Select(p=>p.Name).FirstOrDefault(),//_context.FoodIngredients.Where(p=>p.FoodId==item.Id).Select(p=>p).FirstOrDefault()
                });
			}
            return fods;
		}


        [HttpGet]
        public async Task<ActionResult<List<Fods>>> GetFoods()
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
          setFods();
            return fods;  
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            if (id != food.Id)
            {
                return BadRequest();
            }

            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
          if (_context.Foods == null)
          {
              return Problem("Entity set 'AUTHDBContext.Foods'  is null.");
          }
            _context.Foods.Add(food);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FoodExists(food.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
