using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Features.Users.Commands.ChangePassword;

using MediatR;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IApplicationDbContext context;
    private readonly IPasswordHasher passwordHasher;

    public ChangePasswordHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([request.UserId], cancellationToken);

        if (user == null)
            throw new NotFoundException(typeof(User), request.UserId);

        var isPasswordCorrect = passwordHasher.Verify(request.CurrentPassword, user.HashedPassword);

        if (!isPasswordCorrect)
            throw new BadRequestException("Invalid current password.");

        var newHashedPassword = passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newHashedPassword);
        await context.SaveChangesAsync(cancellationToken);
    }
}