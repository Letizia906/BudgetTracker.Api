using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BudgetTracker.Web.Models;
using System;

namespace BudgetTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentiApiController : ControllerBase
    {
        // 🔹 Percorso corretto verso il file XML del frontend (wwwroot)
        private readonly string xmlPath = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
            "BudgetTracker.Web",
            "wwwroot",
            "movimenti.xml"
        );

        [HttpGet]
        public IActionResult GetAll()
        {
            var movimenti = CaricaMovimenti();
            return Ok(movimenti);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var movimento = CaricaMovimenti().FirstOrDefault(m => m.Id == id);
            if (movimento == null)
                return NotFound();
            return Ok(movimento);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Movimento nuovoMovimento)
        {
            if (nuovoMovimento == null)
                return BadRequest();

            var movimenti = CaricaMovimenti();
            nuovoMovimento.Id = movimenti.Any() ? movimenti.Max(m => m.Id) + 1 : 1;
            movimenti.Add(nuovoMovimento);
            SalvaMovimenti(movimenti);

            return CreatedAtAction(nameof(GetById), new { id = nuovoMovimento.Id }, nuovoMovimento);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movimenti = CaricaMovimenti();
            var movimento = movimenti.FirstOrDefault(m => m.Id == id);
            if (movimento == null)
                return NotFound();

            movimenti.Remove(movimento);
            SalvaMovimenti(movimenti);
            return NoContent();
        }

        private List<Movimento> CaricaMovimenti()
        {
            if (!System.IO.File.Exists(xmlPath))
                return new List<Movimento>();

            var doc = XDocument.Load(xmlPath);
            return doc.Descendants("Movimento")
                .Select(x => new Movimento
                {
                    Id = (int)x.Element("Id"),
                    Data = DateTime.Parse(x.Element("Data")?.Value ?? DateTime.Now.ToString()),
                    Descrizione = (string)x.Element("Descrizione"),
                    Importo = (double)decimal.Parse(x.Element("Importo")?.Value ?? "0"),
                    Entrata = bool.Parse(x.Element("Entrata")?.Value ?? "true"),
                    Categoria = (string?)x.Element("Categoria"),
                    Note = (string?)x.Element("Note"),
                    Tipo = (string?)x.Element("Tipo")
                })
                .ToList();
        }

        private void SalvaMovimenti(List<Movimento> movimenti)
        {
            var doc = new XDocument(
                new XElement("Movimenti",
                    movimenti.Select(m =>
                        new XElement("Movimento",
                            new XElement("Id", m.Id),
                            new XElement("Data", m.Data.ToString("s")),
                            new XElement("Descrizione", m.Descrizione),
                            new XElement("Importo", m.Importo),
                            new XElement("Entrata", m.Entrata),
                            new XElement("Categoria", m.Categoria ?? ""),
                            new XElement("Note", m.Note ?? ""),
                            new XElement("Tipo", m.Tipo ?? "")
                        )
                    )
                )
            );
            doc.Save(xmlPath);
        }
    }
}
