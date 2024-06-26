﻿using Domain.DTO;
using Domain.Parameters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/customers")]
public class CustomerController(IServiceManager serviceManager) : ControllerBase
{
    private readonly IServiceManager _serviceManager = serviceManager;

    [HttpGet(Name = "Customers")]
    public async Task<IActionResult> GetByParameters(Guid customerID, [FromQuery] CustomerParam customerParam, CancellationToken cancellationToken)
    {
        var pagedResult = await _serviceManager.CustomerService.GetByParametersAsync(customerID, customerParam, false, cancellationToken);
        //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagedResult.metaData);
        return Ok(pagedResult.customerDTO);
    }


    [HttpGet("{id:guid}", Name = "CustomerByID")]
    public async Task<IActionResult> GetByID(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _serviceManager.CustomerService.GetByCustomerIDAsync(id, false, cancellationToken);
        return Ok(customer);
    }

    [HttpPost(Name = "CreateCustomer")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Create([FromBody] CustomerDTO customerDTO, CancellationToken cancellationToken)
    {
        var createdCustomer = await _serviceManager.CustomerService.CreateAsync(customerDTO, false, cancellationToken);
        return CreatedAtRoute("CustomerByID", new { id = createdCustomer.CustomerID }, createdCustomer);
    }

    [HttpPut(Name = "UpdateCustomer")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Update([FromBody] CustomerDTO customerDTO, CancellationToken cancellationToken)
    {
        await _serviceManager.CustomerService.UpdateAsync(customerDTO, true, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}", Name = "PartiallyUpdateCustomer")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid id, [FromBody] JsonPatchDocument<CustomerDTO> patchDoc)
    {
        if (patchDoc is null)
            return BadRequest("patchDoc object sent from client is null.");

        var result = await _serviceManager.CustomerService.GetCustomerForPatchAsync(id, true);
        patchDoc.ApplyTo(result.customerToPatch);
        await _serviceManager.CustomerService.SaveChangesForPatchAsync(result.customerToPatch, result.customer);
        return NoContent();
    }

    [HttpDelete("{id:guid}", Name = "DeleteCustomer")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _serviceManager.CustomerService.DeleteAsync(id, false, cancellationToken);
        return NoContent();
    }
}