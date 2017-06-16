namespace TMS.Common.AppManager
{
	/// <summary>
	///     Application Single Instance Scope
	/// </summary>
	public enum AppInstanceScope
	{
		/// <summary>
		///     Only one instance is allowed per Machine (PC)
		///     <para />
		///     <remarks>Registers instance by Environment.MachineName</remarks>
		/// </summary>
		Machine,

		/// <summary>
		///     Only one instance is allowed per Domain on same Machine (PC)
		///     <para />
		///     <remarks>Registers instance by Environment.MachineName + Environment.UserDomainName</remarks>
		/// </summary>
		Domain,

		/// <summary>
		///     Only one instance is allowed per User on same Machine and Domain (PC)
		///     <para />
		///     <remarks>Registers instance by Environment.MachineName + Environment.UserDomainName + Environment.UserName</remarks>
		/// </summary>
		User,
	}
}