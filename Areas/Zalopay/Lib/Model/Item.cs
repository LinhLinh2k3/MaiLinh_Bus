namespace NhaXeMaiLinh.Areas.Zalopay.Lib.Model
{
    public class Item
    {
        public string itemid { get; set; }
        public string itemname { get; set; }
        public long itemprice { get; set; }
        public int itemquantity { get; set; }

        // Constructor
        public Item(string itemid, string itemname, long itemprice, int itemquantity)
        {
            this.itemid = itemid;
            this.itemname = itemname;
            this.itemprice = itemprice;
            this.itemquantity = itemquantity;
        }
    }
}