namespace TasteRestaurant.Data.Models
{
    public class IndevidualButtonPartial
    {
        public string Page { get; set; }
        public string Glyph { get; set; }
        public string ButtonType { get; set; }

        public int? Id { get; set; }

        public string ActionParameters
        {
            get
            {
                if (Id != 0 && Id != null)
                {
                    return Id.ToString();
                }
                else
                {
                    return null;
                }
            }
            set { }
        }
    }
}
