﻿using CleanArch.eCode.Infrastructure.Identity;
using CleanArch.eCode.Infrastructure.Identity.Models;
using CleanArch.eCode.Shared.Notifications;
using CleanArch.eCode.WebApi.SignalR.Services;

namespace CleanArch.eCode.WebApi.SignalR.UseCases;

public class AddNotification : SystemMessage, ICommand<Result>;

internal class AddNotificationHandler(
    AppIdentityDbContext context,
    INotifyService notifier) : ICommandHandler<AddNotification, Result>
{
    public async Task<Result> Handle(AddNotification request, CancellationToken cancellationToken)
    {
        var entity = new Notification
        {
            FromUserId = request.FromUserId,
            FromName = request.FromName,
            ToUserId = request.ToUserId,
            Title = request.Title,
            Message = request.Message,
            Url = request.Url
        };

        await context.Notifications.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // send notify after save record for load notification entries from API when receive
        await notifier.NotifyAsync(request.ToUserId, cancellationToken);

        return Result.Success();
    }
}
