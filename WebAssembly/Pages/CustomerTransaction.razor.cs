﻿using Domain.DTO;
using Domain.Parameters;
using Microsoft.AspNetCore.Components;
using Services.IRepositories;
using WebAssembly.Services;
using WebAssembly.StateManagement;

namespace WebAssembly.Pages;

public partial class CustomerTransaction
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    CustomModalService ConfirmationModalService { get; set; } = default!;
    [Inject]
    CustomNotificationService NotificationService { get; set; } = default!;
    [Inject]
    IServiceManager ServiceManager { get; set; } = default!;
    [Inject]
    CustomerState CustomerState { get; set; } = default!;

    [Parameter] public Guid? ParamCustomerID { get; set; }

    protected string PagePathText = string.Empty;
    protected string FormHeaderText = string.Empty;
    protected GlobalEnum.FormStatus FormStatus = GlobalEnum.FormStatus.New;
    protected bool IsSaving = false;
    protected CustomerParam CustomerParameter = new();

    protected override async Task OnParametersSetAsync()
    {
        if (ParamCustomerID is not null)
        {
            CustomerState.Customer = await ServiceManager.CustomerService.GetCustomerByID((Guid)ParamCustomerID);

            PagePathText = GlobalEnum.FormStatus.Edit.ToString();
            FormHeaderText = $"{GlobalEnum.FormStatus.Edit.ToString()} Existing Customer";
            FormStatus = GlobalEnum.FormStatus.Edit;
        }
        else
        {
            PagePathText = GlobalEnum.FormStatus.New.ToString();
            FormHeaderText = $"Create {GlobalEnum.FormStatus.New.ToString()} Customer";
            FormStatus = GlobalEnum.FormStatus.New;
        }
    }

    public void EvBackToPrevious()
    {
        NavigationManager.NavigateTo($"customer");
    }

    public async Task SubmitAsync(CustomerDTO customer)
    {
        bool confirmationStatus = await ConfirmationModalService.SavingConfirmation("Customer");
        if (!confirmationStatus)
            return;

        IsSaving = true;
        StateHasChanged();

        if (FormStatus == GlobalEnum.FormStatus.New)
        {
            customer.CustomerID = null;
            var response = await ServiceManager.CustomerService.Create(customer);
            if (response.IsSuccessStatusCode)
                NotificationService.SaveNotification("A new customer added");
        }
        else if (FormStatus == GlobalEnum.FormStatus.Edit)
        {
            var response = await ServiceManager.CustomerService.Update(customer);
            if (response.IsSuccessStatusCode)
            {
                NotificationService.SaveNotification("Customer updated");
            }
        }

        //Load customer state after making changes
        await CustomerState.LoadCustomers();

        IsSaving = false;
    }

    public void ClearField()
    {
        Console.WriteLine("Clear fields");
    }
}
