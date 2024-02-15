using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using MovieStore.Application.Interfaces;

namespace MovieStore.Hubs;
public class OrderHub : Hub
{
    private readonly IBranchService _branchService;

    public OrderHub (IBranchService branchService)
    {
        _branchService = branchService;
    }
    public override async Task OnConnectedAsync() 
    {
        if (!Context.User.Identity.IsAuthenticated) Context.Abort();
        else {

        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (string.IsNullOrEmpty(userId)) Context.Abort();
        else {

        if (!Guid.TryParse(userId, out var userGuid)) Context.Abort();
        else {

        var branch = await _branchService.GetByUser(userGuid);
        if (branch == null) Context.Abort();
        else {

        await Groups.AddToGroupAsync(Context.ConnectionId, branch.Id.ToString());

        await base.OnConnectedAsync();
        }}}}
    }
}