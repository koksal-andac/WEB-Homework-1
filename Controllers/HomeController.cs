using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var collectedOrders = Repository.GetAll()
            .Where(o => o.Collected)
            .ToList();

        ViewData["Title"] = "Ana Sayfa";
        ViewBag.CafeteriaName = "Sakarya University Cafeteria";
        ViewBag.CurrentDate = DateTime.Today.ToString("dd.MM.yyyy");
        ViewBag.CollectedCount = collectedOrders.Count;

        return View(collectedOrders);
    }

    public IActionResult AllOrders()
    {
        ViewData["Title"] = "All Orders";

        return View(Repository.GetAll());
    }

    public IActionResult Details(string? id)
    {
        ViewData["Title"] = "Delivery Information";

        MealOrder? order = null;
        if (!string.IsNullOrWhiteSpace(id))
        {
            order = Repository.FindById(id);
        }

        if (order == null)
        {
            ViewBag.Message = string.IsNullOrWhiteSpace(id)
                ? "Order ID not specified"
                : $"Order with '{id}' was not found.";
        }

        return View(order);
    }
    
    [HttpGet]
    public IActionResult Collect()
    {
        ViewData["Title"] = "Collect Meal";

        return View(new CollectionForm());
    }

    [HttpPost]
    public IActionResult Collect(CollectionForm form)
    {
        ViewData["Title"] = "Collect Meal";
 
        var orderId = (form.OrderID ?? string.Empty).Trim();

        if (orderId.Length == 0)
        {
            ViewBag.Message = "Please enter an order ID.";
            ViewBag.MessageType = "error";
            return View(form);
        }

        var order = Repository.FindById(orderId);

        if (order == null)
        {
            ViewBag.Message = $"Order '{orderId}' was not found.";
            ViewBag.MessageType = "error";  
            return View(form);
        }

        if (order.Collected)
        {
            ViewBag.Message = $"Order #{order.OrderID} has already been collected " +
                  $"({order.CollectedAt:dd.MM.yyyy HH:mm}).";
            ViewBag.MessageType = "error"; 
            return View(form);
        }

        order.Collected = true;  
        order.CollectedAt = DateTime.Now;

        ViewBag.Message = $"Order #{order.OrderID} ({order.StudentName} - {order.MealName}) has been collected.";
        ViewBag.MessageType = "success";

        form.OrderID = string.Empty;
        return View(form);
    }
}