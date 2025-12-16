namespace CardCore.Constants
{
    public enum EOrderStatus
    {
        Pending = 1,
        Complete,
        Delivered,
        Cancelled,
        Returned,
        Refunded
    }
    public static class OrderStatusHelper
    {
        public static string GetBadgeClass(EOrderStatus status)
        {
            return status switch
            {
                EOrderStatus.Pending => "bg-warning text-dark fw-bold",
                EOrderStatus.Complete => "bg-success text-white fw-bold",
                EOrderStatus.Delivered => "bg-success text-white fw-bold",
                EOrderStatus.Cancelled => "bg-danger text-white fw-bold",
                EOrderStatus.Returned => "bg-danger text-white fw-bold",
                EOrderStatus.Refunded => "bg-info text-white fw-bold",
                _ => "bg-secondary text-white",
            };
        }
    }
}
