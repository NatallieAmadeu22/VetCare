using System.ComponentModel.DataAnnotations;

namespace VetCare.Models;

public class Animal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do animal.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a espécie.")]
    public string Especie { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome do tutor.")]
    public string Tutor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o telefone do tutor.")]
    public string Telefone { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; } = DateTime.Today;
}
