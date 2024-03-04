using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieStore.Api.Data;
using MovieStore.Api.MappingProfiles;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;
using MovieStore.Hubs;
using Newtonsoft.Json;

namespace MovieStore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly IHubContext<OrderHub> _orderHubContext;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService,IHubContext<OrderHub> orderHubContext)
    {
        _logger = logger;
        _orderService = orderService;
        _orderHubContext = orderHubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderRequest model)
    {
        _logger.LogInformation($"Order received: {JsonConvert.SerializeObject(model)}");

        var dbOrder = Mapper.Map<OrderRequest, Order>(model);
        dbOrder.Description = JsonConvert.SerializeObject(model.Movies);
        var result = await _orderService.Add(dbOrder);
        //var result = new { Succeeded = true, Errors = ""};

        /*
        *    Process payment using Payment Gateaway
        *    Payment and notification should be in a different endpoint
        *    but I keep it here for simplicity for now
        */

        var paymentSucceed = true;
        if (!paymentSucceed)
        {
        //return BadRequest(result.Errors);
            return BadRequest(
                new OrderResponse
                {
                    Text = "Payment failed. Please, try again",
                    Request = model
                });
        }
        
        dbOrder.PaymentSucceed = true;
        await _orderService.Update(dbOrder);

        await _orderHubContext.Clients.Group(model.BranchId.ToString()).SendAsync("Order received", JsonConvert.SerializeObject(model.Movies));

        return Ok(
            new OrderResponse
            {
                Text = "Order processed successfully",
                Request = model
            });

    }
}