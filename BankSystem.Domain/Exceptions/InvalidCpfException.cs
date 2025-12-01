namespace BankSystem.Domain.Exceptions;

public class InvalidCpfException(string message) : InvalidOperationException(message);