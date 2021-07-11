using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Finds Celestial objects by id.
        /// </summary>
        /// <param name="id">The Id of the celestial object</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();

            var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            celestialObject.Satellites = satellites;

            return Ok(celestialObject);
        }

        /// <summary>
        /// Get celestial objects by name.
        /// </summary>
        /// <param name="name">Name of the celestial object</param>
        /// <returns>All the celestial object having the name searched by</returns>
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name.Equals(name)).ToList();

            foreach (var celestialObject in celestialObjects)
            {
                var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            if (celestialObjects.Count == 0)
                return NotFound();

            return Ok(celestialObjects);
        }

        /// <summary>
        /// Gets all celestial objects.
        /// </summary>
        /// <returns>Celestail objects</returns>
        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.Select(x => x).ToList();

            if (celestialObjects.Count == 0)
                return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                var satellites = celestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = satellites;
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);

            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingCelestialObject = _context.CelestialObjects.Find(id);

            if (existingCelestialObject == null)
                return NotFound();

            existingCelestialObject.Name = celestialObject.Name;
            existingCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();
            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            var referencedCelestialObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            if (celestialObject == null && referencedCelestialObjects.Count == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(referencedCelestialObjects);
            _context.CelestialObjects.Remove(celestialObject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
