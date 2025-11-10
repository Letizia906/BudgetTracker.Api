using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BudgetTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentiController : ControllerBase
    {
        private static List<Movimento> movimenti = new List<Movimento>
        {
            new Movimento { Id = 1, Descrizione = "Stipendio", Importo = 1500, Entrata = true },
            new Movimento { Id = 2, Descrizione = "Spesa alimentare", Importo = 80, Entrata = false }
        };

        [HttpGet]
        public IActionResult Get() => Ok(movimenti);

        [HttpPost]
        public IActionResult Post(Movimento nuovo)
        {
            nuovo.Id = movimenti.Count + 1;
            movimenti.Add(nuovo);
            return CreatedAtAction(nameof(Get), new { id = nuovo.Id }, nuovo);
        }
    }

    public class Movimento
    {
        public int Id { get; set; }
        public string Descrizione { get; set; }
        public double Importo { get; set; }
        public bool Entrata { get; set; }
        public DateTime Data { get; internal set; }
        public string? Categoria { get; internal set; }
        public string? Note { get; internal set; }
        public string? Tipo { get; internal set; }
    }
}
