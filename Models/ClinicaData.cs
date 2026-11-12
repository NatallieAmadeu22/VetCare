namespace VetCare.Models;

public static class ClinicaData
{
    public static List<Animal> Animais { get; } =
    [
        new Animal
        {
            Id = 1,
            Nome = "Mel",
            Especie = "Cachorro",
            Tutor = "Ana Souza",
            Telefone = "(11) 99999-1111",
            DataNascimento = new DateTime(2020, 5, 10)
        },
        new Animal
        {
            Id = 2,
            Nome = "Mingau",
            Especie = "Gato",
            Tutor = "Carlos Lima",
            Telefone = "(11) 98888-2222",
            DataNascimento = new DateTime(2022, 8, 20)
        }
    ];

    public static List<Consulta> Consultas { get; } =
    [
        new Consulta
        {
            Id = 1,
            AnimalId = 1,
            AnimalNome = "Mel",
            Veterinario = "Dra. Mariana",
            Data = DateTime.Now.AddDays(2).Date.AddHours(10),
            Motivo = "Consulta de rotina"
        }
    ];
}
