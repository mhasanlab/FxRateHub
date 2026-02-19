namespace FxRateHub.Application.Features.Admin.DTOs;

public record AppSettingDto(
    int Id,
    string Key,
    string Value,
    string? Description
);
