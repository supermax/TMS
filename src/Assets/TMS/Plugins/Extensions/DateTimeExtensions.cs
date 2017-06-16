#region

using System;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	///     DateTime Extensions
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		///     Gets the age.
		/// </summary>
		/// <param name="dateOfBirth"> The date of birth. </param>
		/// <returns> </returns>
		public static int GetAge(this DateTime dateOfBirth)
		{
			var age = DateTime.Now.Year - dateOfBirth.Year;
			if (dateOfBirth.AddYears(age) > DateTime.Now)
			{
				age = age - 1;
			}
			return age;
		}

		/// <summary>
		///     Gets the age.
		/// </summary>
		/// <param name="dateOfBirth">The date of birth.</param>
		/// <returns></returns>
		public static int? GetAge(this DateTime? dateOfBirth)
		{
			if (dateOfBirth == null) return null;
			return dateOfBirth.Value.GetAge();
		}
	}
}