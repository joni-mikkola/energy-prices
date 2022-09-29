using Microsoft.AspNetCore.Mvc;
using EnergyPricesDB;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using EnergyPricesDB.Models;
using System.Collections.Generic;
using EnergyPricesBL;

namespace EnergyPricesWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;
    private readonly EnergyPricesDBContext _context;
    private readonly SlackClient _slackClient;

    public ApiController(EnergyPricesDBContext context, ILogger<ApiController> logger, SlackClient slackClient)
    {
        _context = context;
        _logger = logger;
        _slackClient = slackClient;
    }

    [HttpGet("existing-dates")]
    public IActionResult GetExistingDates()
    {
        try
        {
            var content = _context.NordicElectricityPrices.Select(x => x.Date.ToString("yyyy/MM/dd"));
            return Ok(content);
        } catch(Exception exception)
        {
            _logger.LogError("Failed to get existing dates", exception);
            _slackClient.PostError("Failed to get existing dates", exception);
            return BadRequest();
        }
    }

    [HttpGet("chartdata")]
    public IActionResult GetChartData(DateTime date)
    {
        try
        {
            var data = _context.NordicElectricityPrices.SingleOrDefault(x => x.Date.Date == date);
            var prices = JsonConvert.DeserializeObject<List<PriceModel>>(data.Content);
            return Ok(new
            {
                data = prices.Select(x => x.last),
                labels = prices.Select(x => x.date.ToString("MMM-yy")),
                products = prices.Select(x => x.id)
            });
        } catch (Exception exception)
        {
            _logger.LogError("Failed to get chart data", exception);
            _slackClient.PostError("Failed to get chart data", exception);
            return BadRequest();
        }
        
    }
}
