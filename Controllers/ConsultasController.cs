using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Models;

namespace VetCare.Controllers;

[Authorize]
public class ConsultasController : Controller
{
    private static readonly string[] Veterinarios =
    [
        "Dra. Mariana",
        "Dr. Rafael",
        "Dra. Camila"
    ];

    public IActionResult Index()
    {
        var consultas = ClinicaData.Consultas
            .OrderBy(c => c.Data)
            .ToList();

        return View(consultas);
    }

    public IActionResult Create()
    {
        PrepararAnimais();
        return View(new Consulta());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Consulta consulta)
    {
        var animal = ClinicaData.Animais.FirstOrDefault(a => a.Id == consulta.AnimalId);
        if (animal is null)
        {
            ModelState.AddModelError(nameof(consulta.AnimalId), "Escolha um animal cadastrado.");
        }

        if (!ModelState.IsValid)
        {
            PrepararAnimais();
            return View(consulta);
        }

        consulta.Id = ClinicaData.Consultas.Count == 0
            ? 1
            : ClinicaData.Consultas.Max(c => c.Id) + 1;
        consulta.AnimalNome = animal!.Nome;
        ClinicaData.Consultas.Add(consulta);

        return RedirectToAction(nameof(Index));
    }

    private void PrepararAnimais()
    {
        ViewBag.Animais = new SelectList(ClinicaData.Animais, "Id", "Nome");
        ViewBag.Veterinarios = new SelectList(Veterinarios);
    }
}
