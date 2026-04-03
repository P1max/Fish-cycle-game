namespace Core.Game
{
    public class StoreLot
    {
        public string FishId { get; }
        public float Quality { get; }
        public int Price { get; }
        public bool IsPurchased { get; private set; }
        
        public bool IsVisible { get; set; } 

        public StoreLot(string fishId, float quality, int price, bool isVisible)
        {
            FishId = fishId;
            Quality = quality;
            Price = price;
            IsPurchased = false;
            IsVisible = isVisible;
        }

        public void MarkAsPurchased() => IsPurchased = true;
    }
}