using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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

            var satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            celestialObject.Satellites = satellites;

            if (celestialObject == null)
                return NotFound();

            return Ok(celestialObject);
        }

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
    }
}
