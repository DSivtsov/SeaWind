namespace Application.DtoTime.Tester;

/// <summary>
/// TesterDto транспортный объект (data transfer)
/// сейчас по составу полей совпадает с Tester (но Tester со временем может измениться)
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Age"></param>
public record TesterDto(Guid Id, string Name, int Age);

