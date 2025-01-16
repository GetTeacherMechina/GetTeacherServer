using GetTeacher.Server.Models.Payment;
using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.Networking;
using GetTeacher.Server.Services.Managers.Interfaces.Payment;
using GetTeacher.Server.Services.Managers.Interfaces.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetTeacher.Server.Controllers.Payment;

[ApiController]
[Route("/api/v1/payment/credits")]
public class CreditPaymentController(IWebSocketSystem webSocketSystem, IUserCreditManager userCreditManager, IUserManager userManager,IPaymentManager paymentManager, IItemPriceQuerier  itemPriceQuerier) : ControllerBase
{
	private readonly IWebSocketSystem webSocketSystem = webSocketSystem;
	private readonly IUserCreditManager userCreditManager = userCreditManager;
	private readonly IUserManager userManager = userManager;
	private readonly IPaymentManager paymentManager = paymentManager;
	private readonly IItemPriceQuerier itemPriceQuerier = itemPriceQuerier;

	[HttpGet]
	public async Task<IActionResult> GetCreditPrices()
	{
		return Ok(new ItemPricesResponseModel { CreditPrices = await itemPriceQuerier.GetCreditPricesInDollars() });
	}

	[HttpPost("intent")]
	[Authorize]
	public async Task<IActionResult> MakeCreditsPaymentIntent([FromBody] BuyItemRequestModel buyCreditsRequestModel)
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest("User not found.");

		double? price = await itemPriceQuerier.GetItemPriceInDollars(ItemType.Credits, buyCreditsRequestModel.ItemId);
		if (price is null)
			return BadRequest("No such item found.");

		PaymentDetailsModel paymentDetailsModel = new PaymentDetailsModel(price.Value);
		string? clientSecret = await paymentManager.MakePaymentIntentAsync(paymentDetailsModel);
		if (clientSecret is null)
			return BadRequest("Error creating payment intent.");

		return Ok(new ItemPaymentIntentResponseModel { Status = "Success", ClientSecret = clientSecret });
	}

	[HttpPost("pay")]
	[Authorize]
	public async Task<IActionResult> ConfirmCreditsPaymentIntent([FromBody] ItemPaymentIntentRequestModel itemPaymentIntentRequestModel)
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest("User not found.");

		PaymentResultModel paymentResult = await paymentManager.ConfirmPaymentIntentAsync(itemPaymentIntentRequestModel.PaymentIntentId);
		if (paymentResult.Success)
		{
			await userCreditManager.AddCreditsToUser(user, paymentResult.DollarAmount);
			await webSocketSystem.SendAsync(user.Id, new { AddCreditAmount = paymentResult.DollarAmount }, "UpdateCredits");

			return Ok(new ItemPaymentResultResponseModel { Status = "Payment successful." });
		}
		else
			return Ok(new ItemPaymentResultResponseModel { Status = "Something went wrong." });
	}
}