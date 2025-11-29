using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Common;

/// <summary>
/// Specifies a minimum value constraint.
/// </summary>
public sealed class MinAttribute : ValidationAttribute
{
	private readonly RangeAttribute _rangeAttribute;


	/// <summary>
	/// Constructor that takes an integer minimum value.
	/// </summary>
	/// <param name="minimum">The minimum value, inclusive.</param>
	public MinAttribute(int minimum)
	{
		_rangeAttribute = new RangeAttribute(minimum, int.MaxValue);
	}

	/// <summary>
	/// Constructor that takes a double minimum value.
	/// </summary>
	/// <param name="minimum">The minimum value, inclusive.</param>
	public MinAttribute(double minimum)
	{
		_rangeAttribute = new RangeAttribute(minimum, double.MaxValue);
	}

	/// <inheritdoc/>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var isValid = _rangeAttribute.IsValid(value);
		var memberName = validationContext.MemberName;

		return isValid
			? ValidationResult.Success
			: new ValidationResult($"{memberName} is smaller than the minimum value of {_rangeAttribute.Minimum}");
	}

	public override bool IsValid(object? value) => _rangeAttribute.IsValid(value);
}