namespace SmartWineRack.Data.Dto
{
    public class BottleDto
    {

        private string? upcCode;

        public string UpcCode
        {
            get => upcCode ?? throw new ArgumentNullException(nameof(UpcCode));

            set => upcCode = value ?? throw new ArgumentNullException(nameof(UpcCode));
        }

        public int Slot { get; set; }
    }
}
