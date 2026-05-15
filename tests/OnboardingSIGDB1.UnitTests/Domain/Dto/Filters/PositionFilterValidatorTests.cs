using FluentAssertions;
using FluentValidation.TestHelper;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Common.Filters.Validators;

namespace OnboardingSIGDB1.UnitTests.Domain.Dto.Filters;

public class PositionFilterValidatorTests
{
    private readonly PositionFilterValidator _validator = new();

    [Fact]
    public void Description_TooLong_ShouldHaveError()
    {
        var filter = new PositionFilter { Description = new string('a', 101) };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("The description filter cannot exceed 100 characters.");
    }

    [Fact]
    public void Description_Valid_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = "Developer" };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_TooShort_ShouldHaveError()
    {
        var filter = new PositionFilter { Description = "A" }; // 1 char, trimmed >= 2 required
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("The description filter must have at least 2 characters if provided.");
    }

    [Fact]
    public void Description_Null_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = null };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_Empty_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = "" };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_WhiteSpace_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = "   " };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_ExactlyTwoChars_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = "AB" };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_Exactly100Chars_ShouldNotHaveError()
    {
        var filter = new PositionFilter { Description = new string('a', 100) };
        var result = _validator.TestValidate(filter);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
    
    [Fact]
    public void PageNumber_Zero_ShouldHaveError()
    {
        var filter = new PositionFilter { PageNumber = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageSize_Zero_ShouldHaveError()
    {
        var filter = new PositionFilter { PageSize = 0 };
        var result = _validator.TestValidate(filter);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}

