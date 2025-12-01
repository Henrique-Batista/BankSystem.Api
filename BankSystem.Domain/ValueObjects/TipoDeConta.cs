namespace BankSystem.Domain.ValueObjects;

public enum TipoDeConta
{
    /// <summary>
    /// Represents a checking account, typically used for general transactions
    /// such as deposits, withdrawals, and transfers.
    /// </summary>
    Corrente = 0,
    /// <summary>
    /// Represents a savings account, typically used for storing funds
    /// and accruing interest over time.
    /// </summary>
    Poupanca,
    /// <summary>
    /// Represents a salary-type account, typically used for paying salaries.
    /// </summary>
    Salario,
    /// <summary>
    /// Represents a payment-type account, typically used for paying bills.
    /// </summary>
    Pagamento,
    /// <summary>
    /// Represents a digital-type account, typically used for paying bills.
    /// </summary>
    Digital
}