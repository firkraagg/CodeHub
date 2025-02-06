using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using CodeHub.Services;
using Microsoft.AspNetCore.Components;

namespace CodeHub.Components.Pages;

public partial class ProblemDetails
{
    [Parameter] public int ProblemId { get; set; }

    private Problem? _problem;
    private List<Tag> _tags = new();

    protected override async Task OnInitializedAsync()
    {
        _problem = await ProblemService.GetProblemByIdAsync(ProblemId);
        _tags = await TagService.GetTagsForProblemAsync(_problem.Id);
    }
}