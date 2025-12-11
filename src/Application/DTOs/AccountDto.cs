namespace KontoApi.Application.DTOs;

public class AccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BudgetsCount { get; set; }
}