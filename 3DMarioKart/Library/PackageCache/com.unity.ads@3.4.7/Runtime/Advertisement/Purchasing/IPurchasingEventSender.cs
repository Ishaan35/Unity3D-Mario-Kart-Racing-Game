namespace UnityEngine.Advertisements.Purchasing
{
    internal interface IPurchasingEventSender
    {
        void SendPurchasingEvent(string payload);
    }
}
