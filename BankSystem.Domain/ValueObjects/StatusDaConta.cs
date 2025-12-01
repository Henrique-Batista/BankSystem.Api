namespace BankSystem.Domain.ValueObjects;

public enum StatusDaConta
{
    /// <summary>
    /// Represents the "Active" state of an account in the system.
    /// </summary>
    /// <remarks>
    /// Indicates that the account is active and available for operations such as transactions, deposits, and withdrawals.
    /// An account with "Active" status can be used normally, provided it meets other validation requirements
    /// of the system.
    /// </remarks>
    Ativo = 0,

    /// <summary>
    /// Represents the "Inactive" state of an account in the system.
    /// </summary>
    /// <remarks>
    /// Indicates that the account is deactivated and cannot be used for operations such as transactions, deposits, or withdrawals.
    /// This status might be used to temporarily or permanently disable an account, ensuring it cannot participate in any system activity.
    /// </remarks>
    Inativo
}