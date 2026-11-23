using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterAPI.Models;
using ShelterAPI.Models.DTOs;

namespace ShelterAPI.Controllers
{
    [ApiController]
    [Route("animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public AnimalsController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpPost("newAnimal")]
        public async Task<IActionResult> NewAnimal([FromBody] NewAnimalDto dto)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(dto.ShelterId);
                if (shelter == null)
                    return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });

                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Species = dto.Species,
                    Age = dto.Age,
                    Weight = dto.Weight,
                    ShelterId = dto.ShelterId,
                    RegTime = DateTime.Now
                };

                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Az állat sikeresen rögzítve.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat rögzítése adatbázis-ütközés miatt nem sikerült.", result = (object?)null });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Váratlan hiba történt az állat rögzítésekor.", result = (object?)null });
            }
        }

        [HttpGet("allAnimal")]
        public async Task<IActionResult> AllAnimal()
        {
            try
            {
                var animals = await _context.Animals.ToListAsync();
                if (animals.Count == 0)
                    return NotFound(new { message = "Még nincs rögzített állat.", result = (object?)null });
                return Ok(new { message = "Az állatok listája.", result = animals });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Hiba történt az állatok lekérdezésekor.", result = (object?)null });
            }
        }

        [HttpGet("byId")]
        public async Task<IActionResult> ById([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "Ilyen azonosítójú állat nem található.", result = (object?)null });
                return Ok(new { message = "Az állat adatai.", result = animal });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Hiba történt az állat lekérdezésekor.", result = (object?)null });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] UpdateAnimalDto dto)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "A módosítandó állat nem található.", result = (object?)null });

                var shelter = await _context.Shelters.FindAsync(dto.ShelterId);
                if (shelter == null)
                    return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });

                animal.Name = dto.Name;
                animal.Species = dto.Species;
                animal.Age = dto.Age;
                animal.Weight = dto.Weight;
                animal.ShelterId = dto.ShelterId;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Az állat adatai sikeresen módosítva.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat módosítása adatbázis-ütközés miatt nem sikerült.", result = (object?)null });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Váratlan hiba történt az állat módosításakor.", result = (object?)null });
            }
        }

        [HttpDelete("deleteAnimal")]
        public async Task<IActionResult> DeleteAnimal([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "A törlendő állat nem található.", result = (object?)null });

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Az állat sikeresen törölve.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat törlése adatbázis-ütközés miatt nem sikerült.", result = (object?)null });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Váratlan hiba történt az állat törlésekor.", result = (object?)null });
            }
        }
    }
}
