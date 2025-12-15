using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Features.Users.Commands.ChangePassword;

using MediatR;

public class ChangePasswordHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    : IRequestHandler<ChangePasswordCommand>
{
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