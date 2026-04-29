using FluentAssertions;
using FluentValidation.TestHelper;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.Filters.Validators;

namespace OnboardingSIGDB1.UnitTests.Domain.Dto.Filters;

public class CompanyFilterValidatorTests
{
    private readonly CompanyFilterValidator _validator = new();
    
    [Fact]
    public void PageNumber_Zero_ShouldHaveError()
    {
        var filter = new CompanyFilter { PageNumber = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than 0.");
    }

    [Fact]
    public void PageNumber_Exceeds10000_ShouldHaveError()
    {
        var filter = new CompanyFilter { PageNumber = 10001 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must not exceed 10000.");
    }

    [Fact]
    public void PageNumber_Valid_ShouldNotHaveError()
    {
        var filter = new CompanyFilter { PageNumber = 1 };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageSize_Zero_ShouldHaveError()
    {
        var filter = new CompanyFilter { PageSize = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 100.");
    }

    [Fact]
    public void PageSize_Exceeds100_ShouldHaveError()
    {
        var filter = new CompanyFilter { PageSize = 101 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 100.");
    }

    [Fact]
    public void PageSize_Valid_ShouldNotHaveError()
    {
        var filter = new CompanyFilter { PageSize = 50 };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void FoundedUntil_BeforeFoundedIn_ShouldHaveError()
    {
        var filter = new CompanyFilter
        {
            FoundedIn = new DateTime(2020, 6, 1),
            FoundedUntil = new DateTime(2020, 1, 1)
        };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.FoundedUntil)
            .WithErrorMessage("The 'Founded Until' date must be greater than or equal to 'Founded In'.");
    }

    [Fact]
    public void FoundedUntil_EqualToFoundedIn_ShouldNotHaveError()
    {
        var date = new DateTime(2020, 6, 1);
        var filter = new CompanyFilter { FoundedIn = date, FoundedUntil = date };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.FoundedUntil);
    }

    [Fact]
    public void FoundedIn_Null_FoundedUntil_Set_ShouldNotHaveRangeError()
    {
        var filter = new CompanyFilter { FoundedUntil = DateTime.UtcNow.AddDays(-1) };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.FoundedUntil);
    }

    [Fact]
    public void FoundedIn_TooOld_ShouldHaveError()
    {
        var filter = new CompanyFilter { FoundedIn = new DateTime(1752, 12, 31) };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.FoundedIn)
            .WithErrorMessage("The 'Founded In' date must be after 01/01/1753.");
    }

    [Fact]
    public void FoundedIn_Valid_ShouldNotHaveError()
    {
        var filter = new CompanyFilter { FoundedIn = new DateTime(2000, 1, 1) };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.FoundedIn);
    }

    [Fact]
    public void FoundedIn_Null_ShouldNotHaveError()
    {
        var filter = new CompanyFilter { FoundedIn = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.FoundedIn);
    }

    [Fact]
    public void FoundedUntil_InTheFuture_ShouldHaveError()
    {
        var filter = new CompanyFilter { FoundedUntil = DateTime.UtcNow.AddDays(1) };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.FoundedUntil)
            .WithErrorMessage("The 'Founded Until' date cannot be in the future.");
    }

    [Fact]
    public void FoundedUntil_Null_ShouldNotHaveError()
    {
        var filter = new CompanyFilter { FoundedUntil = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.FoundedUntil);
    }
}

