using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Analytics.Parameters;
using Analytics.Schemas;
using Constants;
using Prime31;
using UnityEngine;

namespace Analytics
{
	public class IAPTransactionEventHelper : EventHelper
	{
		public static TransactionSchema VirtualCurrencyTransaction(string productId, string receipt, string signature)
		{
			bool flag = IsGas(productId);
			Stats stats = EventHelper.GetStats();
			return new TransactionSchema(new TransactionNameParameter((!flag) ? Transaction.BUY_JOULES : Transaction.BUY_GAS), new TransactionTypeParameter(TransactionTypeParameter.Type.PURCHASE), new UserSkillParameter(stats.skill), new UserLevelParameter(stats.level), new IsGuestAccountParameter(stats.guest), null, null, new VirtualCurrencyParameters[1] { VirtualCurrencyReceived(productId) }, null, RealCurrencySpent(productId), null, new TransactionIDParameter(TransactionID(EventHelper.SessionId, productId, receipt)), new ProductIDParameter(productId), new TransactionReceiptSignatureParameter(signature), new TransactionReceiptParameter(receipt));
		}

		public static IAPPurchaseValidatedSchema PurchaseValidated(string productId, string receipt)
		{
			Stats stats = EventHelper.GetStats();
			return new IAPPurchaseValidatedSchema(new TransactionIDParameter(TransactionID(EventHelper.SessionId, productId, receipt)), new IsGuestAccountParameter(stats.guest), new ProductIDParameter(productId), RealCurrencySpent(productId), VirtualCurrencyReceived(productId));
		}

		public static IAPPurchaseInvalidatedSchema PurchaseInvalidated(string productId, string receipt, string signature)
		{
			Stats stats = EventHelper.GetStats();
			return new IAPPurchaseInvalidatedSchema(new TransactionIDParameter(TransactionID(EventHelper.SessionId, productId, receipt)), new TransactionReceiptParameter(receipt), new TransactionReceiptSignatureParameter(signature), new IsGuestAccountParameter(stats.guest), new ProductIDParameter(productId), RealCurrencySpent(productId), VirtualCurrencyReceived(productId));
		}

		public static IAPPurchaseStartedSchema PurchaseStarted(string productId)
		{
			Stats stats = EventHelper.GetStats();
			return new IAPPurchaseStartedSchema(new IsGuestAccountParameter(stats.guest), new ProductIDParameter(productId), RealCurrencySpent(productId), VirtualCurrencyReceived(productId));
		}

		public static IAPPurchaseFailedSchema PurchaseFailed(string productId, string error)
		{
			Stats stats = EventHelper.GetStats();
			return new IAPPurchaseFailedSchema(new IAPPurchaseErrorParameter(error), new IsGuestAccountParameter(stats.guest), new ProductIDParameter(productId), RealCurrencySpent(productId), VirtualCurrencyReceived(productId));
		}

		public static RealCurrencyParameters RealCurrencySpent(string productID)
		{
			int amount = 0;
			RealCurrencyTypeParameter.CurrencyType type = RealCurrencyTypeParameter.CurrencyType.USD;
			GoogleSkuInfo storeProduct = Store.Instance.getStoreProduct(productID);
			if (storeProduct != null)
			{
				amount = CurrencyAmount(storeProduct.price);
				type = TypeFromString(storeProduct.priceCurrencyCode);
			}
			return new RealCurrencyParameters(new RealCurrencyAmountParameter(amount), new RealCurrencyTypeParameter(type));
		}

		private static RealCurrencyTypeParameter.CurrencyType TypeFromString(string currencyCode)
		{
			RealCurrencyTypeParameter.CurrencyType currencyType = (RealCurrencyTypeParameter.CurrencyType)(int)Enum.Parse(typeof(RealCurrencyTypeParameter.CurrencyType), currencyCode);
			if (Enum.IsDefined(typeof(RealCurrencyTypeParameter.CurrencyType), currencyType))
			{
				Debug.Log("Parsed currency code as " + currencyType);
				return currencyType;
			}
			return RealCurrencyTypeParameter.CurrencyType.USD;
		}

		public static VirtualCurrencyParameters VirtualCurrencyReceived(string productId)
		{
			if (!IsGas(productId) && !IsJoules(productId))
			{
				return new VirtualCurrencyParameters();
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary = ServiceManager.Instance.GetGooglePlayProducts();
			int amount = dictionary[productId];
			VirtualCurrencyNameParameter.CurrencyName currencyName = ((!IsGas(productId)) ? VirtualCurrencyNameParameter.CurrencyName.JOULES : VirtualCurrencyNameParameter.CurrencyName.GAS);
			return new VirtualCurrencyParameters(new VirtualCurrencyAmountParameter(amount), new VirtualCurrencyNameParameter(currencyName), new VirtualCurrencyTypeParameter(currencyName));
		}

		private static string TransactionID(string sessionId, string productId, string receipt)
		{
			return EventHelper.Hash(sessionId + productId + receipt);
		}

		private static bool IsGas(string productId)
		{
			return productId.Contains("gascan");
		}

		private static bool IsJoules(string productId)
		{
			return productId.Contains("joules");
		}

		private static int CurrencyAmount(string price)
		{
			return int.Parse(JustDigits(price));
		}

		private static string JustDigits(string price)
		{
			return Regex.Replace(price, "\\D", string.Empty);
		}
	}
}
