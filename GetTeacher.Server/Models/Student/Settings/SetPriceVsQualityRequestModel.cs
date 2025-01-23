using System.ComponentModel.DataAnnotations;

namespace GetTeacher.Server.Models.Student.Settings;

public class SetPriceVsQualityRequestModel
{
	[Range(0, 100)] public required int PriceVsQuality { get; set; }
}