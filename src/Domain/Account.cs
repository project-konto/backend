namespace KontoApi.Domain;

public class Account
{
   public Guid Id { get; private set; }
   public User User { get; private set; }
   public Budget[] Budgets { get; private set; }

   public Account(User user, Budget[] budgets)
   {
      throw new NotImplementedException();
   }
}