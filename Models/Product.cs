namespace DAM.Models
{
  public class Product : DBObject
  {
    public int id { get; set; }
    public string name { get; set; }

    public Product(int id, string name) {
      this.id = id;
      this.name = name;
    }

    public Product(){}
  }
}
