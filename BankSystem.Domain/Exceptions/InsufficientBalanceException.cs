namespace BankSystem.Domain.Exceptions;

public class InsufficientBalanceException(string message) : InvalidOperationException(message);