using MassTransit;

using System;
using System.Threading.Tasks;

namespace EstMTramsit.Contracts
{
	public interface UpdateCustomerAddress
	{
		Guid CommandId { get; }
		DateTime Timestamp { get; }
		string CustomerId { get; }
		string HouseNumber { get; }
		string Street { get; }
		string City { get; }
		string State { get; }
		string PostalCode { get; }
	}


	public interface IOrderPaid
	{
		Guid EventId { get; }
		string Text { get; }
		DateTime Date { get; }
	}

	public interface IUpdateOrderStatus
	{
		Guid CommandId { get; }
		string Text { get; }
		DateTime Date { get; }
	}
}
