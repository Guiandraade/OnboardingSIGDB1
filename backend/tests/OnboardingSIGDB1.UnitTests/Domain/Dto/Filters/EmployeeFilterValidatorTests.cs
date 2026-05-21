using FluentAssertions;
using FluentValidation.TestHelper;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Common.Filters.Validators;

namespace OnboardingSIGDB1.UnitTests.Domain.Dto.Filters;

public class EmployeeFilterValidatorTests
{
    private readonly EmployeeFilterValidator _validator = new();

    [Fact]
    public void HiredUntil_BeforeHiredFrom_ShouldHaveError()
    {
        var filter = new EmployeeFilter
        {
            HiredFrom = new DateTime(2023, 6, 1),
            HiredUntil = new DateTime(2023, 1, 1)
        };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.HiredUntil)
            .WithErrorMessage("The 'Hired Until' date must be greater than or equal to 'Hired From'.");
    }

    [Fact]
    public void HiredUntil_EqualToHiredFrom_ShouldNotHaveError()
    {
        var date = new DateTime(2023, 6, 1);
        var filter = new EmployeeFilter { HiredFrom = date, HiredUntil = date };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.HiredUntil);
    }

    [Fact]
    public void HiredFrom_Null_HiredUntil_Set_ShouldNotHaveRangeError()
    {
        var filter = new EmployeeFilter { HiredUntil = DateTime.UtcNow.AddDays(-1) };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.HiredUntil);
    }

    [Fact]
    public void HiredFrom_TooOld_ShouldHaveError()
    {
        var filter = new EmployeeFilter { HiredFrom = new DateTime(1752, 12, 31) };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.HiredFrom)
            .WithErrorMessage("The start date is invalid.");
    }

    [Fact]
    public void HiredFrom_Valid_ShouldNotHaveError()
    {
        var filter = new EmployeeFilter { HiredFrom = new DateTime(2000, 1, 1) };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.HiredFrom);
    }

    [Fact]
    public void HiredFrom_Null_ShouldNotHaveError()
    {
        var filter = new EmployeeFilter { HiredFrom = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.HiredFrom);
    }
    
    [Fact]
    public void HiredUntil_InTheFuture_ShouldHaveError()
    {
        var filter = new EmployeeFilter { HiredUntil = DateTime.UtcNow.AddDays(1) };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.HiredUntil)
            .WithErrorMessage("The end date cannot be in the future.");
    }

    [Fact]
    public void HiredUntil_Null_ShouldNotHaveError()
    {
        var filter = new EmployeeFilter { HiredUntil = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.HiredUntil);
    }

    [Fact]
    public void Cpf_TooLong_ShouldHaveError()
    {
        var filter = new EmployeeFilter { Cpf = "123456789012345" }; // 15 chars
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("The CPF provided for filtering is too long.");
    }

    [Fact]
    public void Cpf_TooShort_ShouldHaveError()
    {
        var filter = new EmployeeFilter { Cpf = "1234567890" }; // 10 chars
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("The CPF provided is too short.");
    }

    [Fact]
    public void Cpf_Valid11Chars_ShouldNotHaveError()
    {
        var filter = new EmployeeFilter { Cpf = "12345678901" };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Cpf_Null_ShouldNotHaveError()
    {
        var filter = new EmployeeFilter { Cpf = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Cpf_Empty_ShouldNotHaveMinLengthError()
    {
        var filter = new EmployeeFilter { Cpf = "" };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Cpf_WhiteSpace_ShouldNotHaveMinLengthError()
    {
        var filter = new EmployeeFilter { Cpf = "   " };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void PageNumber_Zero_ShouldHaveError()
    {
        var filter = new EmployeeFilter { PageNumber = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageSize_Zero_ShouldHaveError()
    {
        var filter = new EmployeeFilter { PageSize = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

}

