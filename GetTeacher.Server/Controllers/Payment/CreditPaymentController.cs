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
public class CreditPaymentController(IPaymentIntentToCredits paymentIntentToCredits, IWebSocketSystem webSocketSystem, IUserCreditManager userCreditManager, IUserManager userManager,IPaymentManager paymentManager, IItemPriceQuerier  itemPriceQuerier) : ControllerBase
{
	private readonly IPaymentIntentToCredits paymentIntentToCredits = paymentIntentToCredits;
	private readonly IWebSocketSystem webSocketSystem = webSocketSystem;
	private readonly IUserCreditManager userCreditManager = userCreditManager;
	private readonly IUserManager userManager = userManager;
	private readonly IPaymentManager paymentManager = paymentManager;
	private readonly IItemPriceQuerier itemPriceQuerier = itemPriceQuerier;

	[HttpGet]
	public async Task<IActionResult> GetCreditPrices()
	{
		return Ok(new ItemPricesResponseModel { ItemPrices = await itemPriceQuerier.GetCreditPricesInDollars() });
	}

	[HttpPost]
	[Authorize]
	[Route("dev-buy")]
	public async Task<IActionResult> BuyCreditsDev([FromBody] BuyItemRequestModel buyCreditsRequestModel)
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest("User not found.");

		PaymentItemDescriptor? paymentItemDescriptor = await itemPriceQuerier.GetItem(ItemType.Credits, buyCreditsRequestModel.Item.ItemId);
		if (paymentItemDescriptor is null)
			return BadRequest("No such item found.");

		await userCreditManager.AddCreditsToUser(user, paymentItemDescriptor.Amount);
		return Ok(new { });
	}

	[HttpPost("intent")]
	[Authorize]
	public async Task<IActionResult> MakeCreditsPaymentIntent([FromBody] BuyItemRequestModel buyCreditsRequestModel)
	{
		DbUser? user = await userManager.GetFromUser(User);
		if (user is null)
			return BadRequest("User not found.");

		PaymentItemDescriptor? paymentItemDescriptor = await itemPriceQuerier.GetItem(ItemType.Credits, buyCreditsRequestModel.Item.ItemId);
		if (paymentItemDescriptor is null)
			return BadRequest("No such item found.");

		PaymentDetailsModel paymentDetailsModel = new PaymentDetailsModel(paymentItemDescriptor.PriceInDollars);
		PaymentIntendDescriptor? paymentIntendDescriptor = await paymentManager.MakePaymentIntentAsync(paymentDetailsModel);
		if (paymentIntendDescriptor is null)
			return BadRequest("Error creating payment intent.");

		await paymentIntentToCredits.Push(paymentIntendDescriptor.PaymentIntendId, paymentItemDescriptor.Amount);
		return Ok(new ItemPaymentIntentResponseModel { Status = "Success", ClientSecret = paymentIntendDescriptor.ClientSecret });
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
			double? creditAmount = await paymentIntentToCredits.Pop(itemPaymentIntentRequestModel.PaymentIntentId);
			if (creditAmount is null)
				return BadRequest("Payment intent id not found.");

			await userCreditManager.AddCreditsToUser(user, creditAmount.Value);
			await webSocketSystem.SendAsync(user.Id, new { AddCreditAmount = creditAmount.Value }, "UpdateCredits");

			return Ok(new ItemPaymentResultResponseModel { Status = "Payment successful." });
		}
		else
			return Ok(new ItemPaymentResultResponseModel { Status = "Something went wrong." });
	}
}