using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FxRateHub.Domain.Common;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents an application setting key-value pair.
/// </summary>
public class AppSetting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string SettingKey { get; private set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string SettingValue { get; private set; } = string.Empty;

    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? Description { get; private set; }

    [Required]
    public DateTime UpdatedAt { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    private AppSetting() { }

    /// <summary>
    /// Creates a new application setting.
    /// </summary>
    /// <param name="settingKey">The unique key for the setting.</param>
    /// <param name="settingValue">The value of the setting.</param>
    /// <param name="description">Optional description of the setting.</param>
    /// <returns>A new AppSetting instance configured with the provided values.</returns>
    public static AppSetting Create(string settingKey, string settingValue, string? description = null)
    {
        return new AppSetting
        {
            SettingKey = settingKey,
            SettingValue = settingValue,
            Description = description,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the setting value and updates the UpdatedAt timestamp.
    /// </summary>
    /// <param name="newValue">The new value for the setting.</param>
    public void UpdateValue(string newValue)
    {
        SettingValue = newValue;
        UpdatedAt = DateTime.UtcNow;
    }
}
