using System.ComponentModel;

namespace Server.Models
{
    public class PhoneModel
    {
        public int Id { get; set; }

        public int PhoneId { get; set; }

        public Phone Phone { get; set; }

        public double ScreenSize {  get; set; }

        public int BatteryCapacity { get; set; }

        public int CameraResolution { get; set; }

        public int RamId { get; set; }

        public Ram Ram { get; set; }

        public string ChargingPort { get; set; }

        public string OS { get; set; }

        public string CPU  { get; set; }
        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [DefaultValue(0)]
        public int Price { get; set; }

        public int StorageId { get; set; }

        public Storage Storage { get; set; }

        public int ColorId {  get; set; }

        public Color Color { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public PhoneModel()
        {
            Status = true;
        }
    }
}
