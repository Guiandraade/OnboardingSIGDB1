using OnboardingSIGDB1.Domain.Entities.Companies;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class CompanyBuilder
{
    private string _name = "DB1 Group";
    private string _cnpj = "41.977.659/0001-80";
    private DateTime? _foundationDate = new DateTime(2000, 4, 16);

    public static CompanyBuilder New() => new();

    public CompanyBuilder WithName(string name) { _name = name; return this; }
    public CompanyBuilder WithCnpj(string cnpj) { _cnpj = cnpj; return this; }
    public CompanyBuilder WithFoundationDate(DateTime? foundationDate) { _foundationDate = foundationDate; return this; }

    public Company Build() => new(_name, _cnpj, _foundationDate);
}

