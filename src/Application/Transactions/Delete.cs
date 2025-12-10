using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Users.Transactions;

public class DeleteTransactionCommand
{
    public Guid UserId { get; init; }
    public Guid TransactionId { get; init; }
}

public class DeleteTransactionResult
{
    public bool IsDeleted { get; init; }
}

public class DeleteTransactionHandler
{
    private readonly ITransactionRepository transactionRepository;

    public DeleteTransactionHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<DeleteTransactionResult> Handle(DeleteTransactionCommand command)
    {
        var exists = await transactionRepository.ExistsAsync(command.TransactionId);
        if (!exists)
            throw new NotFoundException($"Transaction with id {command.TransactionId} does not exist");

        await transactionRepository.DeleteAsync(command.TransactionId);
        return new()
        {
            IsDeleted = true
        };
    }
}