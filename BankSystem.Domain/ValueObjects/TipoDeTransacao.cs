namespace BankSystem.Domain.ValueObjects;

public enum TipoDeTransacao
{
    /// <summary>
    /// Represents a transaction type specifically for TED (Electronic Funds Transfer).
    /// TED is a common transaction method used for transferring funds between accounts, typically
    /// within a country, with same-day processing.
    /// </summary>
    TED = 0,

    /// <summary>
    /// Represents a transaction type specifically for PIX (Instant Payment System).
    /// PIX is a real-time payment method enabling instant money transfer between accounts,
    /// available 24/7, including weekends and holidays, commonly used in Brazil.
    /// </summary>
    PIX,

    /// <summary>
    /// Represents a transaction type for Boleto (Bank Payment Slip).
    /// Boleto is a popular payment method in Brazil that allows individuals or businesses to make
    /// payments using a predefined slip, either at banks or via online banking, within a specified deadline.
    /// </summary>
    Boleto,

    /// <summary>
    /// Represents a transaction type specifically for a Cheque.
    /// Cheques are paper-based payment instruments that authorize the transfer of an amount from
    /// the issuer's account to the recipient's account. Typically used for non-electronic payments.
    /// </summary>
    Cheque
}