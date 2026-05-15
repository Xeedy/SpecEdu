using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.Integrations;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class IndexModel(IExternalIntegrationService integrationService) : PageModel
{
    public IList<IntegrationEndpointDto> Endpoints { get; set; } = new List<IntegrationEndpointDto>();

    public IList<DataExchangeRecordDto> RecentRecords { get; set; } = new List<DataExchangeRecordDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public string EndpointName { get; set; } = string.Empty;

    [BindProperty]
    public ExternalSystemType EndpointSystemType { get; set; }

    [BindProperty]
    public string? EndpointBaseUrl { get; set; }

    [BindProperty]
    public string? EndpointApiKey { get; set; }

    [BindProperty]
    public bool EndpointIsActive { get; set; } = true;

    public async Task OnGetAsync()
    {
        Endpoints = await integrationService.GetEndpointsAsync();
        var (records, _) = await integrationService.GetExchangeRecordsAsync(pageSize: 20);
        RecentRecords = records;
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (string.IsNullOrWhiteSpace(EndpointName))
        {
            ErrorMessage = "Název endpointu je povinný.";
            return RedirectToPage();
        }

        await integrationService.CreateEndpointAsync(new CreateIntegrationEndpointDto
        {
            Name = EndpointName,
            SystemType = EndpointSystemType,
            BaseUrl = EndpointBaseUrl,
            ApiKey = EndpointApiKey,
            IsActive = EndpointIsActive
        });

        SuccessMessage = $"Endpoint '{EndpointName}' byl vytvořen.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTestConnectionAsync(Guid endpointId)
    {
        var result = await integrationService.TestConnectionAsync(endpointId);

        if (result.Success)
            SuccessMessage = result.Message;
        else
            ErrorMessage = result.Message;

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(Guid endpointId)
    {
        var success = await integrationService.DeactivateEndpointAsync(endpointId);

        if (success)
            SuccessMessage = "Endpoint byl deaktivován.";
        else
            ErrorMessage = "Endpoint nebyl nalezen.";

        return RedirectToPage();
    }
}
