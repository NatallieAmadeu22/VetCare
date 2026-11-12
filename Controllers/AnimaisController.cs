using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.Models;

namespace VetCare.Controllers;

[Authorize]
public class AnimaisController : Controller
{
    public IActionResult Index()
    {
        return View(ClinicaData.Animais);
    }

    public IActionResult Details(int id)
    {
        var animal = ClinicaData.Animais.FirstOrDefault(a => a.Id == id);
        return animal is null ? NotFound() : View(animal);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Animal animal)
    {
        if (!ModelState.IsValid)
        {
            return View(animal);
        }

        animal.Id = ClinicaData.Animais.Count == 0 ? 1 : ClinicaData.Animais.Max(a => a.Id) + 1;
        ClinicaData.Animais.Add(animal);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var animal = ClinicaData.Animais.FirstOrDefault(a => a.Id == id);
        return animal is null ? NotFound() : View(animal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Animal animal)
    {
        if (id != animal.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(animal);
        }

        var animalSalvo = ClinicaData.Animais.FirstOrDefault(a => a.Id == id);
        if (animalSalvo is null)
        {
            return NotFound();
        }

        animalSalvo.Nome = animal.Nome;
        animalSalvo.Especie = animal.Especie;
        animalSalvo.Tutor = animal.Tutor;
        animalSalvo.Telefone = animal.Telefone;
        animalSalvo.DataNascimento = animal.DataNascimento;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var animal = ClinicaData.Animais.FirstOrDefault(a => a.Id == id);
        if (animal is not null)
        {
            ClinicaData.Animais.Remove(animal);
        }

        return RedirectToAction(nameof(Index));
    }
}
