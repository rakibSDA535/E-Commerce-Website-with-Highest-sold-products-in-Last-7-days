using CardCore.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CardCore.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    public AdminOperationsController(IUserOrderRepository userOrderRepository)
    {
        _userOrderRepository = userOrderRepository;
    }

    public async Task<IActionResult> AllOrders()
    {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with id:{orderId} does not found.");
        }
        var orderStatusList = Enum.GetValues(typeof(EOrderStatus))
                .Cast<EOrderStatus>()
                .Select(orderStatus =>
                {
                    return new SelectListItem
                    {
                        Value = ((int)orderStatus).ToString(),
                        Text = orderStatus.ToString()
                    };
                });

        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatus = order.OrderStatus,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatusList = Enum.GetValues(typeof(EOrderStatus))
               .Cast<EOrderStatus>()
               .Select(orderStatus =>
               {
                   return new SelectListItem
                   {
                       Value = ((int)orderStatus).ToString(),
                       Text = orderStatus.ToString(),
                       Selected = orderStatus == data.OrderStatus
                   };
               });
                return View(data);
            }

            await _userOrderRepository.ChangeOrderStatus(data);
            TempData["msg"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            TempData["msg"] = "Something went wrong";
        }
        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
    }


    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        try
        {
            bool isDeleted = await _userOrderRepository.DeleteOrder(orderId);

            if (isDeleted)
            {
                TempData["msg"] = "Order successfully deleted.";
            }
            else
            {
                TempData["msg"] = "Error: Order not found or deletion failed.";
            }
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error deleting order with ID {OrderId}", orderId);
            TempData["msg"] = "An error occurred during deletion.";
        }
        return RedirectToAction(nameof(AllOrders));
    }


}
