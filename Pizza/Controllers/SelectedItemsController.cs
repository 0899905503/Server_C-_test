using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SelectedItemsController : ControllerBase
{
    private static List<Taste> _selectedItems = new List<Taste>();

    // POST api/selecteditems
    [HttpPost]
    public ActionResult Post([FromBody] Taste item)
    {
        if (item == null)
        {
            return BadRequest("Item object is null");
        }

        _selectedItems.Add(item);
        return Ok();
    }

    // GET api/selecteditems
    [HttpGet]
    public ActionResult<IEnumerable<SelectedItem>> Get()
    {
        return Ok(_selectedItems);
    }

}