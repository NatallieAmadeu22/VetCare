using System.ComponentModel.DataAnnotations;

namespace VetCare.Models;

public class Consulta
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string AnimalNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escolha o veterinário.")]
    public string Veterinario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data e o horário.")]
    [DataType(DataType.DateTime)]
    public DateTime Data { get; set; } = DateTime.Now.AddDays(1).Date.AddHours(9);

    [Required(ErrorMessage = "Informe o motivo da consulta.")]
    public string Motivo { get; set; } = string.Empty;
}
