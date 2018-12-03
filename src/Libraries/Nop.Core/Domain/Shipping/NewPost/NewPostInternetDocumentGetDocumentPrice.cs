
namespace Nop.Core.Domain.Shipping.NewPost
{
    public class NewPostInternetDocumentGetDocumentPrice
    {
        public string CitySender { get; set; }
        public string CityRecipient { get; set; }
        public string Weight { get; set; }
        public string ServiceType { get; set; }//"DoorsDoors";
        public string Cost { get; set; }
        public string CargoType { get; set; }//"Cargo";
    }
}
